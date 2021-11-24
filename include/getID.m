function [id] = getID(time_index)
    
    if (time_index < 19.15)
        id = 0;
    else 
        dec = time_index - floor(time_index);
    
        if (dec > 0.15 && dec < 0.65)
            id = 1;
        else
            id = 2;
        end
    end

end
