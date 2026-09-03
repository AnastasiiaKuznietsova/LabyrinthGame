using System;
using System.Media; // Zum Abspielen von Audiodateien verwendet wird

namespace LabyrinthGame.Models
{
    internal class MusicManager
    {
        // Eigenschaft für SoundPlayer
        public SoundPlayer Player { get; private set; }

        // Der Konstruktor erstellt ein SoundPlayer-Objekt, weist es der Eigenschaft Player zu und ruft PlayMusic() auf, um die Musik abzuspielen.
        public MusicManager()
        {
            this.Player = new SoundPlayer("C:\\repos\\LabyrinthGame\\Resources\\Music\\music1.wav");
            //this.PlayMusic();
        }

        // Methode zum Abspielen der Musik
        public void PlayMusic()
        {
            this.Player.Play();
        }
    }
}
