using BergNoten.View;

namespace BergNoten
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("Laden", typeof(Laden));
            Routing.RegisterRoute("Teilnehmer", typeof(Teilnehmer));
            Routing.RegisterRoute("Pruefungen", typeof(Pruefungen));
            Routing.RegisterRoute("Noten", typeof(Noten));
            Routing.RegisterRoute("Exportieren", typeof(Exportieren));
            Routing.RegisterRoute("Einstellungen", typeof(Einstellungen));
            Routing.RegisterRoute("Username", typeof(Username));
            Routing.RegisterRoute("Mustervorlage", typeof(Mustervorlage));
        }
    }
}
