using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    internal class ProgramManager
    {
        // Every ProgramManager manages a ProfileManager and a GameManager
        public ProfileManager ProfileManager { get; set; }
        public MusicManager MusicManager { get; set; }
        public IGameManager GameManager { get; set; }
        public Profile Profile1 { get; set; }
        public Profile Profile2 { get; set; }
        public int BotDifficulty { get; set; }
        public string IpAddressToConnectTo { get; set; }
        public bool SecretMode { get; set; }
        public bool IsBot { get; set; }
        private static ProgramManager _instance;


        public static ProgramManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProgramManager();
                }
                return _instance;
            }
        }

        public ProgramManager()
        {
            this.ProfileManager = new ProfileManager();
            ProfileManager.LoadProfiles();
            ProfileManager.SaveProfiles();
            this.MusicManager = new MusicManager();
            this.BotDifficulty = 0;
            this.SecretMode = false;
            this.IsBot = false;
        }


        // Here a new GameManager object is created to handle a single game between two players (a bot also counts as a player)
        // It needs 2 profiles as an input, that needs to be chosen by the user via the ui
        public void InitializeNewLocalMultiplayerGame()
        {
            this.GameManager = new LocalMultiplayerGameManager(this.Profile1, this.Profile2);
        }

        public void InitializeNewSingleplayerBotGame()
        {
            this.GameManager = new SingleplayerBotGameManager(this.Profile1, this.BotDifficulty);
        }

    }

}
