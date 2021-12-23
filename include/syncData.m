function [syncr] = syncData(DATA)
    
    %DATA must be in the following form:
    %DATA = {IMU_file{:,6}, MITCH_FILE{:,4}, MITCH_FILE{:,4}};
    LEN = length(DATA);
    
    %% TO IMPLEMENT: AUTOMATICALLY GET THE BAND-PASS FILTERING FREQUENCY LOOKING
     % AT FFTs
         %%
%     fs = 100;
%     fN = fs/2;
%     n = length(DATA{1});
%     ff = (0:fs/n:fN);
%     y = fft(DATA{i});
%     
%     myfft = abs(y(1:(ceil((n+1)/2))));
%     plot(ff, myfft);

    %% GET THE SHORTEST LOG
    END = length(DATA{1});
    for i = 2:LEN
        buffer = length(DATA{i});
        if buffer < END
            END = buffer;
        end
    end
    
    %% NORMALIZATION
    for i = 1:LEN
        DATA{i} = DATA{i}/range(DATA{i});
    end
    
    %% FILTERING
    FILTERED_DATA = cell(1, LEN);
    [B,A] = butter(4,[0.02 0.1]);
    for i= 1:LEN
        FILTERED_DATA{i} = filter(B,A,DATA{i});
    end
    
    %% FIND THE VERY MAX
    maxes = zeros(1,LEN);
    for i = 1:LEN
        [~, maxes(i)] = max(FILTERED_DATA{i}(1:END/3));
    end
    
    %% PLOT
%     figure(1)
%     for i = 1:LEN
%         plot(1:END,DATA{i}(1:END));
%         hold on;
%         xline(maxes(i));
%     end
%     hold off; legend(strcat('Data ',num2str((1:i)')))
%     
%     figure(2)
%     for i = 1:LEN
%         plot(1:END,FILTERED_DATA{i}(1:END));
%         hold on;
%         xline(maxes(i));
%     end
%     hold off; legend(strcat('Data ',num2str((1:i)')))
    
    %% SYNCHRONIZE DATA
    l = END - max(maxes);
    syncr = zeros(l,LEN);
    
    for i = 1:LEN
        syncr(:,i) = DATA{i}(maxes(i):l+maxes(i)-1);
    end
    
     %% PLOT SYNCHRONIZED DATA
%     figure(3);
%     for i = 1:LEN
%         plot(1:l,syncr(:,i));
%         hold on;
%     end
%     hold off; legend(strcat('Data ',num2str((1:i)')))
    
     %% ESTIMATE THE TOTAL DELAY
%     [~, latest] =min(maxes);
%     
%     disp("The latest sensor to be turned on is " + num2str(latest));
%     file01{maxes(1),2};
%     (file03{maxes(3),1} - file02{maxes(2),1})/1000
%     % fprintf(num2str(file02{maxes(2),1}), '.16g');
%     % fprintf('\n');
%     % fprintf(num2str(file02{maxes(2)+1,1}), '.16g');
    
     %% PLOT FILTERED AND UNFILTERED DATA
%     
%     for i = 1:LEN
%         figure(i+10);
%         plot(1:length(DATA{i}),DATA{i});
%         hold on;
%         plot(1:length(DATA{i}),FILTERED_DATA{i});
%         xline(maxes(i));
%     end
end