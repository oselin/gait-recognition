classdef myLpmsBT < handle
    
    properties (Constant)
        DEV_NAME = "LPMSB2-9FE157";

        PACKET_ADDRESS0     = 0;
        PACKET_ADDRESS1     = 1;
        PACKET_FUNCTION0    = 2;
        PACKET_FUNCTION1    = 3;
        PACKET_LENGTH0      = 4;
        PACKET_LENGTH1      = 5;
        PACKET_RAW_DATA     = 6;
        PACKET_LRC_CHECK0   = 7;
        PACKET_LRC_CHECK1   = 8;
        PACKET_END          = 9;
        MAX_BUFFER = 4096;
        
        % Command register
        REPLY_ACK             = 0;
        REPLY_NACK            = 1;
        GET_CONFIG            = 4;
        GET_STATUS            = 5;
        GOTO_COMMAND_MODE     = 6;
        GOTO_STREAM_MODE      = 7;
        GET_SENSOR_DATA       = 9;
        
        GET_SERIAL_NUMBER     = 90;
        GET_DEVICE_NAME       = 91;
        GET_FIRMWARE_INFO     = 92;
        
        %Configuration register contents
        LPMS_GYR_AUTOCAL_ENABLED = bitshift(1, 30);
        LPMS_LPBUS_DATA_MODE_16BIT_ENABLED = bitshift(1, 22);
        LPMS_LINACC_OUTPUT_ENABLED = bitshift(1, 21);
        LPMS_DYNAMIC_COVAR_ENABLED = bitshift(1, 20);
        LPMS_ALTITUDE_OUTPUT_ENABLED = bitshift(1, 19);
        LPMS_QUAT_OUTPUT_ENABLED = bitshift(1, 18);
        LPMS_EULER_OUTPUT_ENABLED = bitshift(1, 17);
        LPMS_ANGULAR_VELOCITY_OUTPUT_ENABLED = bitshift(1, 16);
        LPMS_GYR_CALIBRA_ENABLED = bitshift(1, 15);
        LPMS_HEAVEMOTION_OUTPUT_ENABLED = bitshift(1, 14);
        LPMS_TEMPERATURE_OUTPUT_ENABLED = bitshift(1, 13);
        LPMS_GYR_RAW_OUTPUT_ENABLED = bitshift(1, 12);
        LPMS_ACC_RAW_OUTPUT_ENABLED = bitshift(1, 11);
        LPMS_MAG_RAW_OUTPUT_ENABLED = bitshift(1, 10);
        LPMS_PRESSURE_OUTPUT_ENABLED = bitshift(1, 9);
        LPMS_STREAM_FREQ_5HZ_ENABLED      = 0;
        LPMS_STREAM_FREQ_10HZ_ENABLED     = 1;
        LPMS_STREAM_FREQ_25HZ_ENABLED     = 2;
        LPMS_STREAM_FREQ_50HZ_ENABLED     = 3;
        LPMS_STREAM_FREQ_100HZ_ENABLED    = 4;
        LPMS_STREAM_FREQ_200HZ_ENABLED    = 5;
        LPMS_STREAM_FREQ_400HZ_ENABLED    = 6;
        LPMS_STREAM_FREQ_MASK             = 7;
        
        LPMS_STREAM_FREQ_5HZ = 5;
        LPMS_STREAM_FREQ_10HZ = 10;
        LPMS_STREAM_FREQ_25HZ = 25;
        LPMS_STREAM_FREQ_50HZ = 50;
        LPMS_STREAM_FREQ_100HZ = 100;
        LPMS_STREAM_FREQ_200HZ = 200;
        LPMS_STREAM_FREQ_400HZ = 400;
        
        PARAMETER_SET_DELAY = 0.01;
        DATA_QUEUE_SIZE = 64;
    end
   
    properties
        % serial
        BTconn;
        isSensorConnected = false;
        
        % define the properties of the class here, (like fields of a struct)
        rxBuffer = uint8(zeros(1, lpms.MAX_BUFFER));
        rawTxBuffer = uint8(zeros(1, lpms.MAX_BUFFER));
        inBytes = uint8(zeros(1, 2));
        rxState = lpms.PACKET_END;
        rxIndex = 0;
        currentAddress = 0;
        currentFunction = 0;
        currentLength = 0;
        lrcCheck = 0;
        lastTimestamp = 0;
        fps = 0;
        %
        waitForAck = false;
        waitForData = false;
        readCallbackFcnRet = false;
        % Settings related
        imuId = 0;
        gyrRange = 0;
        accRange = 0;
        magRange = 0;
        streamingFrequency = 0;
        filterMode = 0;
        isStreamMode = true;

        configurationRegister = 0;
        configurationRegisterReady = false;
        sensorDataLength = 0;
        serialNumber = 'none';
        serialNumberReady = false;
        deviceName = 'none';
        deviceNameReady = false;
        firmwareInfo= 'none';
        firmwareInfoReady = false;
        firmwareVersion = 'none';

        accEnable = false;
        gyrEnable = false;
        magEnable = false;
        angularVelEnable = false;
        quaternionEnable = false;
        eulerAngleEnable = false;
        linAccEnable = false;
        pressureEnable = false;
        altitudeEnable = false;
        temperatureEnable = false;
        heaveEnable = false;
        sixteenBitDataEnable = false;
        resetTimestampFlag = false;
        
        % sensorData
        sensorData = struct(...
            'timestamp', 0.0, ...
            'gyr',  zeros(1,3), ...
            'acc',  zeros(1,3), ...
            'mag',  zeros(1,3), ...
            'angVel',  zeros(1,3), ...
            'quat',  zeros(1,4), ...
            'euler',  zeros(1,3), ...
            'linAcc',  zeros(1,3), ...
            'pressure', 0.0, ...
            'altitude', 0.0, ...
            'temperature', 0.0, ...
            'heave', 0.0 ...
        );
    
        dataQueue = [];
    end

    methods

        function ret = connect(obj)
            disp("searching for devices...");
            BTlist = bluetoothlist;
            disp("search completed");
            BTdevices = BTlist{:,"Name"};
            name = BTdevices(contains(BTdevices,'LPMSB2'));

            if(isempty(name))
                ret = false;
                disp("device not found");
                return;
            end
            
            disp(name);
            try
                obj.BTconn = bluetooth(name);
            catch err
                disp(err);
            end
            %check if connection is successful
            if(~isempty(obj.BTconn))
                disp("connection successfull");
                obj.isSensorConnected = true;
            else
                disp("connection failed");
                obj.isSensorConnected = false;
                ret = obj.isSensorConnected;
                return;
            end
            %set terminator
            configureTerminator(obj.BTconn,"CR/LF");
        end

        function ret = disconnect(obj)
            delete(obj.BTconn);
            obj.BTconn = [];
            obj.isSensorConnected = false;
            ret = true;
            disp("disconnected");
        end

        function ret = sendData(obj, command, length) %lenght as number of bytes
            %ret true if successfully send data
            %    false + close connection if successful
            txBuffer = zeros(1, 11+length, "uint8");
            txBuffer(1) = 0x3A;
            txBuffer(2:3) = typecast(uint16(obj.imuId), 'uint8');
            txBuffer(4:5) = typecast(uint16(command), 'uint8');
            txBuffer(6:7) = typecast(uint16(length), 'uint8');
            txLrcCheck = 0;
            for i=2:7
                txLrcCheck = txLrcCheck + txBuffer(i);
            end
            for i = 1:length
                txBuffer(7+i) = obj.rawTxData(i);
                txLrcCheck = txLrcCheck + obj.rawTxData(i);
            end
            txBuffer(8+length:9+length) = typecast(uint16(txLrcCheck), 'uint8');
            txBuffer(10+length)=0x0D;
            txBuffer(11+length)=0x0A;
            
            try
                write(obj.BTconn, txBuffer, "uint8");
                ret = true;
            catch
                ret = false;
                obj.disconnect()
                disp("failed to send command, interrupting connection");
            end
        end

        function ret = setCommandMode(obj)
            %send command mode set
            if (~obj.sendData(obj.GOTO_COMMAND_MODE, 0))
                %fail to send message
                disp("failed to set command mode");
                ret = obj.isSensorConnected;
                return;
            end
            
            configureCallback(  obj.BTconn, ...
                                "terminator", ...
                                @obj.readCallbackFcn ...
                             );
            obj.waitForAck = true;  

            %wait for ack
            timeout = 0;
            while (obj.waitForAck && timeout < 100)
                pause(obj.PARAMETER_SET_DELAY);
                timeout = timeout + 1;
            end

            if(obj.waitForAck) %no response
                ret = false;
                disp("no ACK received");
                obj.waitForAck = false;
                return;
            end

            if(obj.readCallbackFcnRet) %ACK received
                disp("ACK received, set command mode successful");
                obj.isStreamMode = false;
                ret = true;
            else %NACK received
                ret = false;
                disp("NACK received, set command mode failed");
            end

%             if(obj.readAck())
%                 ret = true;
%                 disp("set command mode successful");
%                 obj.isStreamMode = false;
%             else
%                 ret = false;
%                 disp("set command mode failed");
%             end            
        end

        function ret = readAck(obj)
            if (~obj.isSensorConnected) 
                ret = false;
                disp("no device connected");
                return;
            end
            configureCallback(  obj.BTconn, ...
                                "byte", ...
                                11, ...
                                @obj.readCallbackFcn ...
                             );
            %wait for response
            timeout = 0;
            obj.waitForAck = true;
            while (obj.waitForAck && timeout < 100)
                pause(obj.PARAMETER_SET_DELAY);
                timeout = timeout + 1;
            end

            if(obj.waitForAck) %no response
                ret = false;
                disp("no ACK received");
                return;
            end
            if(obj.readCallbackFcnRet) %ACK received
                ret = true;
            else %NACK received
                ret = false;
                disp("NACK received");
            end
        end

%         function ret = readData(obj,count)
%             if (~obj.isSensorConnected) 
%                 ret = false;
%                 disp("no device connected");
%                 return;
%             end
%             configureCallback(  obj.serConn, ...
%                                 "byte", ...
%                                 11+count, ...
%                                 @obj.readCallbackFcn(obj.serConn) ...
%                              );
%             %wait for response
%             timeout = 0;
%             obj.waitForData = true;
%             while (obj.waitForData && timeout < 100)
%                 pause(obj.PARAMETER_SET_DELAY);
%                 timeout = timeout + 1;
%             end
%             if(obj.waitForData)
%                 ret = false;
%                 disp("no data received");
%                 return;
%             else
%                 ret = true;
%                 return;
%             end
%         end

        function readCallbackFcn(obj, hObject, eventdata)
            if(obj.isSensorConnected)
                try
                    n = hObject.NumBytesAvailable;
                    disp(n);
                    if (n > 0)
                        data = read(hObject, n, "uint8");
                        obj.readCallbackFcnRet = obj.parse(data, 11+count);
                    else
                        disp("packets have been lost");
                        obj.readCallbackFcnRet = false;
                        return;
                    end
                catch err
                    disp(err);
                    disp("error occured during reading");
                    obj.readCallbackFcnRet = false;
                    return;
                end
            end
        end

        function ret = parse(obj, buf, nBytes)
            for i=1:nBytes
                d = buf(i);
                switch obj.rxState 
                    case obj.PACKET_END
                        if (d == 0x3A)
                            obj.rxState = obj.PACKET_ADDRESS0;
                        else
                            ret = false;
                            if(i==1)
                                disp("error in parsing: header not found")
                            else
                                disp("error in parsing: missing bytes");
                            end
                            return;
                        end

                    case obj.PACKET_ADDRESS0
                       obj.inBytes(1) = d;
                       obj.rxState = obj.PACKET_ADDRESS1;

                    case obj.PACKET_ADDRESS1
                        obj.inBytes(2) = d;
                        obj.currentAddress = typecast(obj.inBytes,'uint16');
                        obj.imuId = obj.currentAddress;
                        obj.rxState = obj.PACKET_FUNCTION0;

                    case obj.PACKET_FUNCTION0
                        obj.inBytes(1) = d;
                        obj.rxState = obj.PACKET_FUNCTION1;

                    case obj.PACKET_FUNCTION1
                        obj.inBytes(2) = d;
                        obj.currentFunction = typecast(obj.inBytes,'uint16');
                        obj.rxState = obj.PACKET_LENGTH0;

                    case obj.PACKET_LENGTH0
                        obj.inBytes(1) = d;
                        obj.rxState = obj.PACKET_LENGTH1;

                    case obj.PACKET_LENGTH1
                        obj.inBytes(2) = d;
                        obj.currentLength = typecast(obj.inBytes,'uint16');
                        obj.rxIndex = 0;
                        obj.rxState = obj.PACKET_RAW_DATA;
                        obj.lrcCheck = obj.currentAddress + obj.currentFunction + obj.currentLength;
                      

                    case obj.PACKET_RAW_DATA
                        if obj.rxIndex == obj.currentLength
                            obj.inBytes(1) = d;
                            obj.rxState = obj.PACKET_LRC_CHECK1; 
                        else
                            if (obj.rxIndex < obj.MAX_BUFFER)
                                obj.rxBuffer(obj.rxIndex+1) = d;
                                obj.rxIndex = obj.rxIndex + 1;
                                obj.lrcCheck = obj.lrcCheck + d;
                            else
                                obj.rxState = obj.PACKET_END;
                                ret = false;
                                disp("error in parsing: error during raw data reading");
                                return;
                            end
                        end

                    case obj.PACKET_LRC_CHECK1
                        obj.inBytes(2) = d;
                        lrcReceived = typecast(obj.inBytes,'uint16');
                        if (lrcReceived == obj.lrcCheck)
                            ret = obj.parseFunction();  
                        else
                            ret = false;
                            disp("error in parsing: LRC doesn't match");
                            return;
                        end
                        obj.rxState = obj.PACKET_END;
                      
                    otherwise 
                        obj.rxState = obj.PACKET_END;
                        ret = false;
                        disp("error in parsing: initial rxState not PACKET_END");
                        return;                        
                end
            end
        end

        function ret = parseFunction(obj)
            switch (obj.currentFunction)
                case obj.REPLY_ACK
%                     disp('REPLY_ACK') 
                    obj.waitForAck = false;
                    ret = true;
                    return;
                    
                case obj.REPLY_NACK
%                     disp('REPLY_NACK') 
                    obj.waitForAck = false;
                    ret = false;
                    return;
            
                case obj.GET_CONFIG
%                     disp('GET_CONFIG') 
                    obj.configurationRegister = obj.convertRxbytesToInt(0, obj.rxBuffer);
                    obj.parseConfig(obj.configurationRegister);
                    obj.waitForData = false;
                    
                case obj.GET_SENSOR_DATA
                    %disp('GET_SENSOR_DATA')
                    t = toc;
                    obj.fps = 1/t;
                    obj.fps;
                    tic
                    obj.parseSensorData();
                    obj.waitForData = false;
                    
                case obj.GET_DEVICE_NAME
%                     disp('GET_DEVICE_NAME')
                    obj.deviceName = obj.convertRxbytesToString(16, obj.rxBuffer);
                    %deviceNameReady = true;
                    obj.waitForData = false;
            end
        end

        
    end
 
end