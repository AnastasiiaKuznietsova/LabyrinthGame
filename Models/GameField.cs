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
    }
}
