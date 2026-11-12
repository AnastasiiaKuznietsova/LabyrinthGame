using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    public class Card : INotifyPropertyChanged
    {
        // Every Card has a unique Id, a Paths Dictionary and a TreasurePaths Dictionary
        public string Id { get; }
        // Schlüssel (int) - bestimmte Position auf der Karte. Wert(HashSet<int>) - alle möglichen Positionen, zu denen man von dieser Position aus gelangen kann.
        public Dictionary<int, HashSet<int>> Paths { get; set; }
        // Schlüssel (string) - Name des Schatzes. Wert(HashSet<int>) - Positionen, an denen der Schatz gefunden werden kann
        public Dictionary<string, HashSet<int>> TreasurePaths { get; set; }
        public string ImagePath { get; set; }

        private double _imageAngle;
        // Diese Eigenschaft speichert und aktualisiert den Winkel des Bildes und benachrichtigt über Änderungen
        public double ImageAngle
        {
            get { return _imageAngle; }
            set
            {
                _imageAngle = value;
                OnPropertyChanged(nameof(ImageAngle));
            }
        }

        // Konstruiert ein Kartenobjekt basierend auf dem Eingabestring und dem Bildpfad, initialisiert Eigenschaften und setzt den Bildwinkel
        public Card(string inputString, string imagePath)
        {
            // Der inputString wird in der Methode GenerateCardStack während der Initialisierung der Karten erstellt
            if (inputString == "empty")
            {
                Id = "empty";
                Paths = new();
                TreasurePaths = new();
                ImagePath = imagePath;
                ImageAngle = 0;
            }
            else
            {
                Id = inputString.Split(";")[0];
                Paths = GeneratePaths(inputString.Split(";")[1]);
                TreasurePaths = UnpackStringForTreasurePaths(inputString.Split(";")[2]);
                ImagePath = imagePath;
                ImageAngle = 0;
            }
        }

        // Kopie alle karten für BotPlayer
        public Card(Card cardToCopy)
        {
            this.Id = cardToCopy.Id;
            this.Paths = cardToCopy.Paths;
            this.TreasurePaths = cardToCopy.TreasurePaths;
            this.ImagePath = cardToCopy.ImagePath;
            this.ImageAngle = cardToCopy.ImageAngle;
        }

        #region Help methods to initialize Card objects (these methods are only needed for creating all the 50 Card-objects)
        private List<HashSet<int>> UnpackStringForPaths(string zippedPaths)
        {

            List<HashSet<int>> outputPathList = [];                               // Initialisiert eine leere Liste von HashSets

            // Teilt den komprimierten String an den Bindestrichen
            foreach (string zippedPath in zippedPaths.Split("-"))                 // Beispiel: "3458-67" wird in ["3458", "67"] aufgeteilt
            {
                HashSet<int> unpackedPath = [];                                   // Initialisiert einen neuen HashSet für den aktuellen Pfad

                // Wandelt jeden Charakter im Pfad in eine Zahl um und fügt sie dem HashSet hinzu
                foreach (char zippedCoordinate in zippedPath)                     // Beispiel: "3458" wird in {3, 4, 5, 8} umgewandelt
                {
                    unpackedPath.Add((int)char.GetNumericValue(zippedCoordinate));// Konvertiert den Charakter in seinen numerischen Wert
                }

                // Fügt den entpackten Pfad der Liste hinzu
                outputPathList.Add(unpackedPath);                                 // Beispiel: outputPathList enthält jetzt [{3, 4, 5, 8}]
            }

            return outputPathList;                                                // Gibt die Liste der entpackten Pfade zurück
        }

        private Dictionary<int, HashSet<int>> GeneratePaths(string zippedPaths)
        {
            // Entpackt den komprimierten String in eine Liste von HashSets
            List<HashSet<int>> pathList = UnpackStringForPaths(zippedPaths);      // z.B pathList = [{3, 4, 5, 8}, {6, 7}]

            Dictionary<int, HashSet<int>> outputPathDictionary = [];              // Initialisiert ein leeres Dictionary

            foreach (HashSet<int> path in pathList)                               // Iteriert durch jeden HashSet in der Liste
            {
                foreach (int fromCoordinate in path)                              // Iteriert durch jede Position im aktuellen Pfad
                {
                    HashSet<int> toCoordinates = [];

                    foreach (int toCoordinate in path.Where(x => x != fromCoordinate)) // Fügt alle anderen Positionen im Pfad zu den Zielpositionen hinzu
                    {
                        toCoordinates.Add(toCoordinate);                           // z.B toCoordinates = {4, 5, 8}
                    }

                    outputPathDictionary[fromCoordinate] = toCoordinates;          // key - fromCoordinate, valye - toCoordinates
                                                                                   // z.B outputPathDictionary[3] = {4, 5, 8} 
                                                                                   // {3: {4, 5, 8}, 4: {3, 5, 8}, 5: {3, 4, 8}, 8: {3, 4, 5}, 6: {7}, 7: {6}}
                }
            }
            return outputPathDictionary;                                           // Gibt das gefüllte Dictionary zurück
        }

        // entpackt einen komprimierten String, der Schatzpfade beschreibt, in ein Dictionary
        private Dictionary<string, HashSet<int>> UnpackStringForTreasurePaths(string zippedTreasurePaths)
        {
            // Initialisiert ein leeres Dictionary zur Speicherung der entpackten Schatzpfade
            Dictionary<string, HashSet<int>> outputTreasurePathDictionary = [];

            // Teilt den komprimierten String an den Pluszeichen, um einzelne Einträge zu erhalten
            foreach (string zippedDictionaryEntry in zippedTreasurePaths.Split("+"))   // z.B: "Ring-3458+Krone-67" wird in ["Ring-3458", "Krone-67"] aufgeteilt
            {
                HashSet<int> unpackedPath = [];                                        // Initialisiert einen neuen HashSet für den aktuellen Pfad

                // Teilt den Eintrag an den Bindestrichen, um die Koordinaten zu erhalten, und wandelt jeden Charakter in eine Zahl um
                foreach (char zippedCoordinate in zippedDictionaryEntry.Split("-")[1]) // z.B:"3458" wird in {3, 4, 5, 8} umgewandelt
                {
                    unpackedPath.Add((int)char.GetNumericValue(zippedCoordinate));
                }
                // Speichert den Schatznamen als Schlüssel und den entpackten Pfad als Wert im Dictionary
                outputTreasurePathDictionary[zippedDictionaryEntry.Split("-")[0]] = unpackedPath; // z.B: "Ring" -> {3, 4, 5, 8}
            }
            return outputTreasurePathDictionary;                                      // Gibt das gefüllte Dictionary zurück
        }
        #endregion

        // Dreht eine gegebene Karte im Uhrzeigersinn
        public void Rotate()
        {
            // Aktualisiert die Pfade und Schatzpfade nach der Drehung, den Bildwinkel und benachrichtigt über die Änderungen
            this.Paths = RotatedPaths(Paths);
            this.TreasurePaths = RotatedTreasurePaths(TreasurePaths);
            this.ImageAngle = (ImageAngle + 90) % 360;
            OnPropertyChanged(nameof(Paths));
            OnPropertyChanged(nameof(TreasurePaths));
            OnPropertyChanged(nameof(ImageAngle));
        }

        // Dreht die Karte eine bestimmte Anzahl von Malen
        public void RotateXManyTimes(int xTimesToRotate)
        {
            for (int i = 1; i <= xTimesToRotate; i++)
            {
                this.Rotate();
            }
        }

        // Dreht die Karte zurück zur Standardrotation (0 Grad)
        public void RotateBackToDefaultRotation()
        {
            while (ImageAngle != 0)
            {
                this.Rotate();
            }
        }

        // Hilfsmethode zum Drehen der Pfade
        private Dictionary<int, HashSet<int>> RotatedPaths(Dictionary<int, HashSet<int>> pathsToRotate)
        {
            Dictionary<int, HashSet<int>> outputRotatedPaths = [];   // Ein leeres Dictionary wird erstellt, um die gedrehten Pfade zu speichern

            // Die Methode iteriert durch jeden Schlüssel(fromCoordinate) im pathsToRotate Dictionary
            foreach (int fromCoordinate in pathsToRotate.Keys.ToList())
            {
                HashSet<int> newToCoordinates = [];                  //Für jede fromCoordinate wird ein neues HashSet newToCoordinates erstellt, um die neuen Zielkoordinaten zu speichern

                foreach (int toCoordinate in pathsToRotate[fromCoordinate])
                {
                    int newToCoordinate = toCoordinate + 2;          // Für jede toCoordinate wird die neue Zielkoordinate (newToCoordinate) berechnet, indem 2 hinzugefügt wird
                    if (newToCoordinate == 9) newToCoordinate = 1;   // Wenn die neue Zielkoordinate 9 ist, wird sie auf 1 gesetzt
                    if (newToCoordinate == 10) newToCoordinate = 2;  // Wenn die neue Zielkoordinate 10 ist, wird sie auf 2 gesetzt
                    newToCoordinates.Add(newToCoordinate);           // Fügt die neue Zielkoordinate dem HashSet newToCoordinates hinzu
                }

                int newFromCoordinate = fromCoordinate + 2;          // Berechnet die neue Ausgangskoordinate (newFromCoordinate), indem 2 hinzugefügt wird
                if (newFromCoordinate == 9) newFromCoordinate = 1;   // Wenn die neue Ausgangskoordinate 9 ist, wird sie auf 1 gesetzt
                if (newFromCoordinate == 10) newFromCoordinate = 2;  // Wenn die neue Ausgangskoordinate 10 ist, wird sie auf 2 gesetzt
                outputRotatedPaths[newFromCoordinate] = newToCoordinates; // Speichert die neuen Zielkoordinaten im Dictionary outputRotatedPaths
            }

            return outputRotatedPaths;                               // Gibt das gefüllte Dictionary zurück
        }

        // Hilfsmethode zum Drehen der Schatzpfade
        private Dictionary<string, HashSet<int>> RotatedTreasurePaths(Dictionary<string, HashSet<int>> pathsToRotate)
        {
            Dictionary<string, HashSet<int>> outputRotatedTreasurePaths = [];// Ein leeres Dictionary wird erstellt, um die gedrehten Schatzpfade zu speichern


            foreach (string treasure in pathsToRotate.Keys.ToList())         // Die Methode iteriert durch jeden Schlüssel (treasure) im pathsToRotate Dictionary
            {
                HashSet<int> newCoordinates = [];                            // Für jeden Schatz wird ein neues HashSet newCoordinates erstellt, um die neuen Koordinaten zu speichern


                foreach (int coordinate in pathsToRotate[treasure])          // Iteriert durch jede Koordinate im aktuellen Schatzpfad
                {
                    int newCoordinate = coordinate + 2;                      // Berechnet die neue Koordinate (newCoordinate), indem 2 hinzugefügt wird
                    if (newCoordinate == 9) newCoordinate = 1;               // Wenn die neue Koordinate 9 ist, wird sie auf 1 gesetzt
                    if (newCoordinate == 10) newCoordinate = 2;              // Wenn die neue Koordinate 10 ist, wird sie auf 2 gesetzt
                    newCoordinates.Add(newCoordinate);                       // Fügt die neue Koordinate dem HashSet newCoordinates hinzu
                }

                outputRotatedTreasurePaths[treasure] = newCoordinates;       // Speichert die neuen Koordinaten im Dictionary outputRotatedTreasurePaths
            }

            return outputRotatedTreasurePaths;                               // Gibt das gefüllte Dictionary zurück
        }

        #region Diese Methoden sind nützlich für die Entwicklung und das Debugging
        // Gibt das gesamte Pfad-Dictionary aus
        public void PrintPaths()
        {
            foreach ((int from, HashSet<int> toPath) in Paths)
            {
                Console.Write($"{from} : {{ ");
                foreach (int toCoordinate in toPath)
                {
                    Console.Write($"{toCoordinate} ");
                }
                Console.WriteLine("}\n");
            }
        }

        // Gibt das gesamte Schatzpfad-Dictionary aus
        public void PrintTreasurePaths()
        {
            foreach ((string treasure, HashSet<int> path) in TreasurePaths)
            {
                Console.Write($"{treasure} : {{ ");
                foreach (int Coordinate in path)
                {
                    Console.Write($"{Coordinate} ");
                }
                Console.WriteLine("}\n");
            }
        }
        #endregion

        // Ereignis für Eigenschaftsänderungen
        public event PropertyChangedEventHandler PropertyChanged;

        // Methode zur Benachrichtigung über Eigenschaftsänderungen
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}