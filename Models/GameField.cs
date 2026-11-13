using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LabyrinthGame.Models
{
    internal class GameField
    {
        // Die GameField-Klasse verwaltet ein 100x100 2D-Array von Karten namens "Field"
        public Card?[,] Field { get; set; }
        public const int xSize = 100;
        public const int ySize = 100;
        public Card? LastPlacedCard { get; set; }

        // Konstruktor, der das Spielfeld initialisiert
        public GameField()
        {
            this.Field = new Card[xSize, ySize];
            this.LastPlacedCard = null;
        }

        // Konstruktor, der ein bestehendes Spielfeld kopiert
        public GameField(GameField gameFieldToCopy)
        {
            this.Field = gameFieldToCopy.Field;
            this.LastPlacedCard = gameFieldToCopy.LastPlacedCard;
        }

        // Gibt die Grenzen des belegten Bereichs zurück
        public (int minX, int maxX, int minY, int maxY) GetOccupiedBounds()
        {
            int minX = xSize, maxX = 0, minY = ySize, maxY = 0;

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (Field[x, y] != null)
                    {
                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                }
            }
            return (minX, maxX, minY, maxY);
        }

        // Gibt die Karte an den angegebenen x-y-Koordinaten zurück
        public Card? GetCard(int x, int y)
        {
            return Field[x, y] ?? null;
            // Der ??-Operator prüft,ob der linke Operand null ist.Wenn Field[x, y] eine Karte enthält (nicht null),wird diese Karte zurückgegeben
        }

        // Weist dem Feld an den Koordinaten (x, y) eine Karte zu, wenn das Feld leer ist.
        public void AddCard(int x, int y, Card card)
        {
            Field[x, y] = Field[x, y] == null ? card : throw new InvalidOperationException($"Cant place card there: x={x} y={y}");
            // Ternärer Operator, der eine Bedingung überprüft. Wenn die Bedingung wahr ist, wird der Wert nach dem ? zurückgegeben
            // Überprüft, ob das Feld an (x, y) null ist. Wenn ja, wird die Karte zugewiesen, sonst wird eine InvalidOperationException ausgelöst
        }

        // Entfernt eine Karte an den angegebenen x-y-Koordinaten
        public void RemoveCard(int x, int y)
        {
            if (Field[x, y] == null)
            {
                // Wenn keine Karte an der Position (x, y) vorhanden ist, wird eine Ausnahme ausgelöst
                throw new InvalidOperationException($"Cant remove card there, because there is no card: x={x} y={y}");
            }
            else
            {
                // Wenn eine Karte vorhanden ist, wird sie entfernt, indem das Feld auf null gesetzt wird
                Field[x, y] = null;
            }
        }

        // Überprüft, ob eine Karte durch Schätze mit einer anderen Karte auf dem Spielfeld verbunden ist
        public bool IsAbleToPickUpAnyCards(Card card)
        {
            // Erstellt eine Liste aller möglichen Karten auf dem Spielfeld, die aufgenommen werden können
            List<Card> allPossibleCardsToPickUp = this.GetAllCards();

            foreach (Card possibleCardToPickUp in allPossibleCardsToPickUp)
            {
                if (this.AreTheseCardsTreasuresConnected(card, possibleCardToPickUp))
                {
                    return true;
                }
            }
            return false;
        }

        // Überprüft, ob eine Karte entfernt werden darf, indem sichergestellt wird, dass die verbleibenden Karten verbunden bleiben
        // Entfernt vorübergehend eine Karte und prüft die Verbindung aller verbleibenden Karten.
        public bool IsAllowedToRemoveCard(Card card)
        {
            var position = this.GetCardCoordinate(card);

            Field[position.Item1, position.Item2] = null;

            bool allConnected = AreAllCardsConnected();

            Field[position.Item1, position.Item2] = card;

            return allConnected;
        }

        // Überprüft, ob alle Karten verbunden sind
        public bool AreAllCardsConnected()
        {
            // Erstellt eine Liste aller (x, y)-Koordinaten und speichert die erste Koordinate mit einer Karte in start. Ist keine Karte vorhanden, bleibt start null
            var start = Enumerable.Range(0, xSize)
                                  // Erstellt eine Sequenz von Ganzzahlen von 0 bis xSize - 1
                                  .SelectMany(x => Enumerable.Range(0, ySize), (x, y) => new { x, y })
                                  // Für jedes x von 0 bis xSize -1 wird eine Sequenz von y - Werten von 0 bis ySize -1 erstellt, die alle möglichen(x, y) - Koordinaten des Spielfelds als flache Sequenz von Objekten darstellt
                                  .FirstOrDefault(pos => Field[pos.x, pos.y] != null);
            // Durchläuft die (x, y)-Sequenz und gibt die erste Koordinate zurück, bei der Field[x, y] nicht null ist. Andernfalls wird null zurückgegeben

            // Wenn keine Karte auf dem Spielfeld ist, sind alle Karten verbunden (leeres Spielfeld)
            if (start == null) return true;

            // Erstellt eine Menge für besuchte Koordinaten, eine Warteschlange für die Breitensuche (BFS), fügt die Startkoordinate hinzu und markiert sie als besuch
            var visited = new HashSet<(int, int)>();
            var queue = new Queue<(int, int)>();
            queue.Enqueue((start.x, start.y));
            visited.Add((start.x, start.y));

            // Solange die Warteschlange nicht leer ist, wird die BFS fortgesetzt
            while (queue.Count > 0)
            {
                // Entnimmt die nächste Koordinate aus der Warteschlange
                var (x, y) = queue.Dequeue();
                // Durchläuft alle Nachbarn der aktuellen Koordinate
                foreach (var (nx, ny) in GetNeighbors(x, y).Where(pos => Field[pos.Item1, pos.Item2] != null && !visited.Contains(pos)))
                {
                    // Markiert den Nachbarn als besucht und fügt ihn zur Warteschlange hinzu
                    visited.Add((nx, ny));
                    queue.Enqueue((nx, ny));
                }
            }

            // Überprüft, ob alle Karten auf dem Spielfeld besucht wurden
            return Enumerable.Range(0, xSize)
                             // Erstellt eine Sequenz von Ganzzahlen von 0 bis xSize - 1
                             .SelectMany(x => Enumerable.Range(0, ySize), (x, y) => new { x, y })
                             // Für jedes x von 0 bis xSize -1 wird eine Sequenz von y - Werten von 0 bis ySize -1 erstellt, die alle möglichen(x, y) - Koordinaten des Spielfelds als flache Sequenz von Objekten darstellt
                             .Where(pos => Field[pos.x, pos.y] != null)
                             // Filtert die Sequenz, um nur die Koordinaten zu behalten, bei denen sich eine Karte auf dem Spielfeld
                             .All(pos => visited.Contains((pos.x, pos.y)));
                             // Prüft, ob alle gefilterten Koordinaten in visited enthalten sind. Gibt true zurück, wenn alle besucht wurden, sonst false
        }

        // Gibt nur die möglichen Nachbarn zurück, ohne zu prüfen, ob an diesen Koordinaten tatsächlich eine Karte vorhanden ist
        public IEnumerable<(int, int)> GetNeighbors(int x, int y)
        {
            if (x > 0) yield return (x - 1, y);
            if (x < xSize - 1) yield return (x + 1, y);
            if (y > 0) yield return (x, y - 1);
            if (y < ySize - 1) yield return (x, y + 1);
        }

        // Sammelt alle gefundene Karten auf dem Spielfeld und gibt sie als Liste zurück
        public List<Card> GetAllCards()
        {
            List<Card> cards = new();
            (int minX, _, int minY, _) = this.GetOccupiedBounds();
            // Die Methode GetOccupiedBounds liefert die minimalen x- und y-Koordinaten der belegten Felder; die anderen Werte werden ignoriert

            for (int x = minX; x < xSize; x++)  // Zwei verschachtelte for-Schleifen durchlaufen die Koordinaten des Spielfelds von minX bis xSize und von minY bis ySize

            {
                for (int y = minY; y < ySize; y++)
                {
                    if (Field[x, y] != null)    // Für jede Koordinate wird überprüft, ob sich dort eine Karte befindet (d.h. das Feld ist nicht null)
                    {
                        cards.Add(Field[x, y]); // Gefundene Karten werden der Liste cards hinzugefügt

                    }
                }
            }

            return cards;                       // Die Liste der gefundenen Karten wird zurückgegeben
        }

        // Überprüft, ob zwei Karten durch Schätze verbunden sind
        public bool AreTheseCardsTreasuresConnected(Card card1, Card card2)
        {
            // Initialisiert eine Liste von besuchten Karten und eine Warteschlange für die Karten, die überprüft werden müssen
            List<Tuple<Card, int>> visited = new();
            Queue<Tuple<Card, List<int>>> queue = new();
            List<int> previousConnections = new();

            // Setzt die Startkarte auf card1 und sammelt alle Schatzpfade der Startkarte
            Card startCard = card1;
            List<string> treasures = startCard.TreasurePaths.Keys.ToList();

            // Überprüft, ob die beiden Karten überhaupt gemeinsame Schätze haben
            if (!startCard.TreasurePaths.Keys.Any(key => card2.TreasurePaths.ContainsKey(key)))
            {
                return false;
            }
            // Überprüft, ob die beiden Karten identisch sind
            if (startCard == card2 || startCard.Id == card2.Id)
            {
                return false;
            }

            // Iteriert über jeden einzigartigen Schatzpfad der Startkarte
            foreach (List<int> treasurePath in startCard.TreasurePaths.Values.Distinct(HashSet<int>.CreateSetComparer())
                                                                             .Select(hashSet => hashSet.ToList()))
            {
                // Setzt die Liste der besuchten Karten zurück und fügt die Startkarte hinzu
                visited.Clear();
                visited.Add((startCard, 1).ToTuple());
                // Aktualisiert die Liste der Schätze, die mit dem aktuellen Schatzpfad übereinstimmen
                treasures = startCard.TreasurePaths.Where(kvp => kvp.Value.ToList()
                                                   .SequenceEqual(treasurePath))
                                                   .Select(kvp => kvp.Key).ToList();
                // Fügt die Startkarte und den aktuellen Schatzpfad zur Warteschlange hinzu
                queue.Enqueue((startCard, treasurePath).ToTuple());

                // Solange die Warteschlange nicht leer ist, überprüft jede Karte und ihre Verbindungen
                while (queue.Count > 0)
                {
                    (Card currentCard, List<int> currentOutgoingConnections) = queue.Dequeue();

                    // Iteriert über die Nachbarn der aktuellen Karte und deren Verbindungen
                    foreach ((Card neighbor, List<int> connections) in GetNeighborsAndConnectionsWherePathsConnected(currentCard, currentOutgoingConnections))
                    {
                        // Überspringt Nachbarn, die bereits viermal besucht wurden
                        if (visited.Any(tuple => tuple.Item1.Equals(neighbor) && tuple.Item2 >= 4))
                        {
                            continue;
                        }
                        else
                        {
                            // Überprüft, ob der Nachbar die Zielkarte ist und ob sie durch einen gemeinsamen Schatz verbunden sind
                            if (neighbor.Equals(card2) && treasures.Any(treasure => neighbor.TreasurePaths.ContainsKey(treasure) && neighbor.TreasurePaths[treasure].Any(value => connections.Contains(value))))

                            {
                                Debug.WriteLine("Successfully removed the card!");
                                return true;
                            }
                            else
                            {
                                // Aktualisiert die Besuchsliste oder fügt den Nachbarn hinzu
                                if (visited.Any(tuple => tuple.Item1.Equals(neighbor)))
                                {
                                    visited = visited.Select(tuple => tuple.Item1.Equals(neighbor) ? new Tuple<Card, int>(tuple.Item1, tuple.Item2 + 1) : tuple).ToList();
                                }
                                else
                                {
                                    visited.Add((neighbor, 1).ToTuple());
                                }

                                // Fügt die neuen Verbindungen zur Warteschlange hinzu
                                List<int> newOutgoingConnections = neighbor.Paths.Where(pathEntry => connections.Contains(pathEntry.Key))
                                                                                 .SelectMany(pathEntry => pathEntry.Value).ToList();
                                queue.Enqueue((neighbor, newOutgoingConnections).ToTuple());
                            }
                        }
                    }

                }
            }
            return false;
        }

        // Gibt die Nachbarn und Verbindungen zurück, bei denen die Pfade verbunden sind
        private Dictionary<Card, List<int>> GetNeighborsAndConnectionsWherePathsConnected(Card card, List<int> connectionsToCheck)
        {
            Dictionary<Card, List<int>> result = new();
            (int x, int y) = this.GetCardCoordinate(card);
            List<int> mainConnectionsToCheck = connectionsToCheck.Select(x => new Dictionary<int, int> { { 1, 6 }, { 2, 5 }, { 3, 8 }, { 4, 7 }, { 5, 2 }, { 6, 1 }, { 7, 4 }, { 8, 3 } }[x]).ToList();

            // Überprüft, ob es einen oberen Nachbarn gibt und ob die Pfade verbunden sind
            if (HasTopNeighbor(x, y)) if (Field[x, y - 1].Paths.Keys.Any(key => mainConnectionsToCheck.Contains(key)))
            {
                connectionsToCheck = mainConnectionsToCheck.Where(x => x == 5 || x == 6).ToList();
                if (connectionsToCheck.Count > 0) result.Add(Field[x, y - 1], Field[x, y - 1].Paths.Keys.Where(key => connectionsToCheck.Contains(key)).ToList());
            }
            // Überprüft, ob es einen unteren Nachbarn gibt und ob die Pfade verbunden sind
            if (HasBottomNeighbor(x, y)) if (Field[x, y + 1].Paths.Keys.Any(key => mainConnectionsToCheck.Contains(key)))
            {
                connectionsToCheck = mainConnectionsToCheck.Where(x => x == 1 || x == 2).ToList();
                if (connectionsToCheck.Count > 0) result.Add(Field[x, y + 1], Field[x, y + 1].Paths.Keys.Where(key => connectionsToCheck.Contains(key)).ToList());
            }
            // Überprüft, ob es einen linken Nachbarn gibt und ob die Pfade verbunden sind
            if (HasLeftNeighbor(x, y)) if (Field[x - 1, y].Paths.Keys.Any(key => mainConnectionsToCheck.Contains(key)))
            {
                connectionsToCheck = mainConnectionsToCheck.Where(x => x == 3 || x == 4).ToList();
                if (connectionsToCheck.Count > 0) result.Add(Field[x - 1, y], Field[x - 1, y].Paths.Keys.Where(key => connectionsToCheck.Contains(key)).ToList());
            }
            // Überprüft, ob es einen rechten Nachbarn gibt und ob die Pfade verbunden sind
            if (HasRightNeighbor(x, y)) if (Field[x + 1, y].Paths.Keys.Any(key => mainConnectionsToCheck.Contains(key)))
            {
                connectionsToCheck = mainConnectionsToCheck.Where(x => x == 7 || x == 8).ToList();
                if (connectionsToCheck.Count > 0) result.Add(Field[x + 1, y], Field[x + 1, y].Paths.Keys.Where(key => connectionsToCheck.Contains(key)).ToList());
            }

            return result;
        }

        // Gibt die Koordinaten der Karte zurück
        public Tuple<int, int> GetCardCoordinate(Card card)
        {
            (int minX, _, int minY, _) = this.GetOccupiedBounds();

            // Durchläuft das Spielfeld, um die Karte zu finden
            for (int x = minX; x < xSize; x++)
            {
                for (int y = minY; y < ySize; y++)
                {
                    if (Field[x, y] != null)
                    {
                        if (Field[x, y] == card || Field[x, y].Id == card.Id)
                        {
                            return (x, y).ToTuple();
                        }
                    }
                }
            }

            return null;
        }


        #region Platziert?
        // Gibt eine Liste von Tupeln (Koordinaten) zurück, an denen Karten im Feld platziert werden könnten
        public List<Tuple<int, int>> GetAllPossibleNeighborCoordinates()
        {
            List<Tuple<int, int>> outputCoordinateList = [];

            // Durchläuft das Spielfeld, um mögliche Nachbarpositionen zu finden
            for (int x = 0; x < 99; x++)
            {
                for (int y = 0; y < 99; y++)
                {
                    if (GetCard(x, y) == null && HasNeighbor(x, y))
                    {
                        outputCoordinateList.Add(Tuple.Create(x, y));
                    }
                }
            }

            return outputCoordinateList;
        }

        // Gibt eine Liste von Tupeln (Koordinaten) zurück, an denen eine bestimmte Karte im Feld platziert werden kann
        public List<Tuple<int, int>> GetAllPossibleCoordinatesWhereCardCanBePlaced(Card card)
        {
            List<Tuple<int, int>> outputCoordinateList = [];

            // Durchläuft alle möglichen Nachbarpositionen, um zu überprüfen, ob die Karte platziert werden kann
            foreach ((int x, int y) in GetAllPossibleNeighborCoordinates())
            {
                if (IsAllowedToBePlaced(x, y, card))
                {
                    outputCoordinateList.Add(Tuple.Create(x, y));
                }
            }

            return outputCoordinateList;
        }

        // Überprüft, ob eine bestimmte Karte an einer x-y-Koordinate im Feld platziert werden kann
        private Boolean IsAllowedToBePlaced(int x, int y, Card card)
        {
            // Überprüft, ob die Karte oben platziert werden kann
            if (HasTopNeighbor(x, y))
            {
                if (card.Paths.ContainsKey(1) && GetCard(x, y - 1).Paths.ContainsKey(6) || card.Paths.ContainsKey(2) && GetCard(x, y - 1).Paths.ContainsKey(5)) return true;
            }
            // Überprüft, ob die Karte unten platziert werden kann
            if (HasBottomNeighbor(x, y))
            {
                if (card.Paths.ContainsKey(6) && GetCard(x, y + 1).Paths.ContainsKey(1) || card.Paths.ContainsKey(5) && GetCard(x, y + 1).Paths.ContainsKey(2)) return true;
            }
            // Überprüft, ob die Karte rechts platziert werden kann
            if (HasRightNeighbor(x, y))
            {
                if (card.Paths.ContainsKey(3) && GetCard(x + 1, y).Paths.ContainsKey(8) || card.Paths.ContainsKey(4) && GetCard(x + 1, y).Paths.ContainsKey(7)) return true;
            }
            // Überprüft, ob die Karte links platziert werden kann
            if (HasLeftNeighbor(x, y))
            {
                if (card.Paths.ContainsKey(8) && GetCard(x - 1, y).Paths.ContainsKey(3) || card.Paths.ContainsKey(7) && GetCard(x - 1, y).Paths.ContainsKey(4)) return true;
            }

            return false;
        }
        #endregion

        #region Nachbarn?
        // Überprüft, ob sich eine Karte oben, unten, links oder rechts der angegebenen x-y-Koordinate befindet
        private Boolean HasNeighbor(int x, int y)
        {
            if (x <= 0 || y <= 0) return false;
            return Field[x + 1, y] != null || Field[x - 1, y] != null || Field[x, y + 1] != null || Field[x, y - 1] != null ? true : false;
        }

        // Überprüft, ob sich eine Karte oben an der angegebenen x-y-Koordinate befindet
        private Boolean HasTopNeighbor(int x, int y)
        {
            if (x <= 0 || y <= 0) return false;
            return Field[x, y - 1] != null ? true : false;
        }

        // Überprüft, ob sich eine Karte unten an der angegebenen x-y-Koordinate befindet
        private Boolean HasBottomNeighbor(int x, int y)
        {
            if (x <= 0 || y <= 0) return false;
            return Field[x, y + 1] != null ? true : false;
        }

        // Überprüft, ob sich eine Karte rechts an der angegebenen x-y-Koordinate befindet
        private Boolean HasRightNeighbor(int x, int y)
        {
            if (x <= 0 || y <= 0) return false;
            return Field[x + 1, y] != null ? true : false;
        }

        // Überprüft, ob sich eine Karte links an der angegebenen x-y-Koordinate befindet
        private Boolean HasLeftNeighbor(int x, int y)
        {
            if (x <= 0 || y <= 0) return false;
            return Field[x - 1, y] != null ? true : false;
        }
        #endregion

    }
}