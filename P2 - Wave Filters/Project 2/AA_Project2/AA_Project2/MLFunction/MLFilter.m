function MLFilter(WavIn, WavOut, type, fc)
[x fs] = audioread(WavIn);
N = 8;
if(type == 0)  %lowpass
    [num den] = butter(N, fc/(fs/2));
else           %highpass
    [num den] = butter(N, fc/(fs/2), 'high');
end
y = filter(num, den, x);
audiowrite(WavOut, y, fs);
return;