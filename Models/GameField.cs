using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
