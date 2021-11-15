function Visualizzazione_accelerazioni(activities,acceleration,acc_id,time,direction)

% Visualizzare i dati grezzi suddivisi per attività. 
% Nello specifico, assegnare un colore diverso ad ogni attività e visualizzare i dati sullo stesso grafico. 
% Realizzo un ciclo for per processare ogni singola attività separatamente.
% Nello specifico:
%   Isolo la singola attività
%   Sostituisco la variabile "NaN" nelle posizioni del vettore con id diverso dall'attività in esame (sia per il vettore di tempo che per quello di accelerazione)
%   Plot sullo stesso grafico delle attività

% NOTA: Accelerazione in posizione sdraiato = 0 perchè il device è ruotato. Si ottiene un valore di 9.8 lungo l'asse z.

% Numero di azioni presenti nei dati caricati (riferiti al singolo
% soggetto)
n_act = unique(acc_id);
figure
col = ['r' 'k' 'g' 'b' 'c' 'm'];

for i = 1:length(n_act)
    time_p = time; % Creo vettore dei tempi specifico per l'attività
    acceleration_p = acceleration; % Creo vettore di accelerazione specifico per l'attività
    time_p(acc_id ~= n_act(i)) = NaN; % Imposto NaN in corrispondenza di id_attività diverso da quello che sto considerando. Evito così il plot di valori nulli
    acceleration_p(acc_id ~= n_act(i)) = NaN;
    plot(time_p, acceleration_p, 'Color', col(i), 'LineWidth', 1.2) % Plot del dato con colore definito nel vettore "col"
    hold on
end
if strlength(direction)==1
    title (['Accelerazione asse ' direction]);
else
    title(direction);
end
xlabel 'Tempo [s]'
ylabel 'Accelerazione [m\cdot s^{-2}]'
legend (activities, 'Location', 'southwestoutside')
grid on

hold off;
end

