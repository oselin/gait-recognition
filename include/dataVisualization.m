

function []= dataVisualization(videoName, startingPoint, labeledData)
    
    %% Import the video
    myvideo = VideoReader(videoName,"CurrentTime",startingPoint);

    %% Get frame dimensions
    [height, width] = size(read(myvideo, 1),1,2);
    
    %% Set colored filters
    temp = ones(height, width);
    mylayers = {
                cat(3, 1.00*temp, 1.00*temp, 000*temp), ... %PHASE 1
                cat(3, 1.00*temp, 0.65*temp, 000*temp), ... %PHASE 2
                cat(3, 1.00*temp, 0.00*temp, 000*temp), ... %PHASE 3
                cat(3, 0.00*temp, 0.50*temp, 000*temp), ... %PHASE 4
        };

    
    %% Get the total labeld time
    last = labeledData{end, 2};

    %% Display the video
    if myvideo.Duration < last
        disp("Warning: data overshoots the video");
    end

    for i = 1:last
        frame = readFrame(myvideo);
        hold on;
        myimage = imshow(frame);
        filter = imshow(mylayers{labeledData(i+1918,1)+1});
        filter.AlphaData = 0.2;
        hold off;
        title(sprintf('Current Time = %.3f sec', myvideo.CurrentTime));
        pause(1/myvideo.FrameRate);
    end
end