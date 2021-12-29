flatten_result = cell(54,1);

for i = 1:I
    for j = 1:J
        for k = 1:K
            for l = 1:L
                flatten_result{(i-1)*J*K*L+(j-1)*K*L+(k-1)*L+l} = results{i,j,k,l};
            end
        end
    end
end

for i = 1:(I*J*K*L)
    disp(flatten_result{i}.phaseAcc);
    disp(flatten_result{i}.testAcc);
    disp(flatten_result{i}.streamAcc);
end
