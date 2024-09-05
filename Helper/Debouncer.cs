namespace Bergnoten.Helper;


/// <summary>
/// Der Debouncer startet einen Timer, sobald die Mehtode Debounce aufgerufen wird. Sollte der Timer schon laufen 
/// beginnt dieser wieder von vorne. Erst nachdem der Timer sein Ziel erreicht hat, wird der Task ausgeführt.
/// </summary>
/// <param name="delayInMilliseconds"></param>
class Debouncer(int delayInMilliseconds)
{
    private CancellationTokenSource _cancellationTokenSource = new();

    public async Task Debounce(Func<Task> action)
    {
        // Jede vorherige Aufgabe abbrechen
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            // Warte die angegebene Verzögerung
            await Task.Delay(delayInMilliseconds, token);
            if (!token.IsCancellationRequested)
            {
                await action(); // Führe die Aktion aus
            }
        }
        catch (TaskCanceledException)
        {
            // Task wurde abgebrochen, nichts tun
        }
    }
}
