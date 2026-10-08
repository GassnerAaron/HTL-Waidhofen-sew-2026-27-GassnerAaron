using System;

interface IFigur
{
    double berechneOberflaeche();
    double berechneVolumen();
}

abstract class Figur : IFigur
{
    public string Beschreibung { get; }
    public Figur Next { get; set; }
    public Figur Previous { get; set; }

    public Figur(string beschreibung)
    {
        Beschreibung = beschreibung;
    }

    public abstract double berechneOberflaeche();
    public abstract double berechneVolumen();

    public override string ToString()
    {
        return $"{Beschreibung} (Oberflaeche={berechneOberflaeche():F2}, Volumen={berechneVolumen():F2})";
    }
}

class Kugel : Figur
{
    public double Radius { get; }

    public Kugel(string beschreibung, double radius) : base(beschreibung)
    {
        Radius = radius;
    }

    public override double berechneOberflaeche()
    {
        return 4 * Math.PI * Radius * Radius;
    }

    public override double berechneVolumen()
    {
        return 4.0 / 3 * Math.PI * Radius * Radius * Radius;
    }
}

class Wuerfel : Figur
{
    public double Seitenlaenge { get; }

    public Wuerfel(string beschreibung, double a) : base(beschreibung)
    {
        Seitenlaenge = a;
    }

    public override double berechneOberflaeche()
    {
        return 6 * Seitenlaenge * Seitenlaenge;
    }

    public override double berechneVolumen()
    {
        return Seitenlaenge * Seitenlaenge * Seitenlaenge;
    }
}

class DoppeltVerketteteFigurListe
{
    private Figur first;
    private Figur last;
    public int Count { get; private set; }

    // 0: Am naeheren Ende beginnen. Der erste Index ist 0.
    private Figur GetAt(int index)
    {
        if (index < 0 || index >= Count)
        {
            return null;
        }

        if (index < Count / 2)
        {
            Figur current = first;
            for (int i = 0; i < index; i++)
            {
                current = current.Next;
            }
            return current;
        }
        else
        {
            Figur current = last;
            for (int i = Count - 1; i > index; i--)
            {
                current = current.Previous;
            }
            return current;
        }
    }

    // 1: Hinten einfuegen. fig muss eine neue oder abgetrennte Figur sein.
    public void Add(Figur fig)
    {
        if (fig == null) return;

        fig.Next = null;
        fig.Previous = last;

        if (last != null)
            last.Next = fig;
        else
            first = fig; // Die Liste war leer.

        last = fig;
        Count++;
    }

    // 2: Jede Figur besuchen, ohne die Pfeile zu veraendern.
    public void AusgabeVorwaerts()
    {
        Figur current = first;
        while (current != null)
        {
            Console.WriteLine(current);
            current = current.Next;
        }
    }

    public void AusgabeRueckwaerts()
    {
        Figur current = last;
        while (current != null)
        {
            Console.WriteLine(current);
            current = current.Previous;
        }
    }

    // 3: Vorne einfuegen. Erst verbinden, dann first aendern!
    public void Push(Figur fig)
    {
        if (fig == null) return;

        fig.Previous = null;
        fig.Next = first;

        if (first != null)
            first.Previous = fig;
        else
            last = fig; // Die Liste war leer.

        first = fig;
        Count++;
    }

    // 4: Erste Figur merken, entfernen und zurueckgeben.
    public Figur Pop()
    {
        if (first == null) return null;

        Figur ergebnis = first;
        first = first.Next;

        if (first != null)
            first.Previous = null;
        else
            last = null; // Die einzige Figur wurde entfernt.

        ergebnis.Next = null;
        ergebnis.Previous = null;
        Count--;
        return ergebnis;
    }

    // 5: In der Mitte muessen vier Pfeile gesetzt werden.
    public void InsertAt(int index, Figur fig)
    {
        if (fig == null) return;

        if (index <= 0)
        {
            Push(fig);
            return;
        }
        if (index >= Count)
        {
            Add(fig);
            return;
        }

        Figur nach = GetAt(index);
        Figur vor = nach.Previous;

        fig.Previous = vor;
        fig.Next = nach;
        vor.Next = fig;
        nach.Previous = fig;
        Count++;
    }

    // 6: Erst suchen, dann die Nachbarn direkt verbinden.
    public void Remove(int index)
    {
        Figur fig = GetAt(index);
        if (fig == null) return;
        Remove(fig);
    }

    // Hilfsmethode: fig stammt aus dieser Liste, deshalb keine Suche.
    private void Remove(Figur fig)
    {
        if (fig.Previous != null)
            fig.Previous.Next = fig.Next;
        else
            first = fig.Next;

        if (fig.Next != null)
            fig.Next.Previous = fig.Previous;
        else
            last = fig.Previous;

        fig.Next = null;
        fig.Previous = null;
        Count--;
    }

    // 7: Bei jeder Figur Next und Previous tauschen.
    public void Umdrehen()
    {
        Figur current = first;
        while (current != null)
        {
            Figur tmp = current.Next; // Alten Weg zum Rest merken!
            current.Next = current.Previous;
            current.Previous = tmp;
            current = tmp;
        }

        Figur alterAnfang = first;
        first = last;
        last = alterAnfang;
    }

    // 8: vor <-> a <-> b <-> rest wird vor <-> b <-> a <-> rest.
    public void VertauscheBenachbarte()
    {
        Figur a = first;
        while (a != null && a.Next != null)
        {
            Figur vor = a.Previous;
            Figur b = a.Next;
            Figur rest = b.Next;

            if (vor != null)
                vor.Next = b;
            else
                first = b;
            b.Previous = vor;

            b.Next = a;
            a.Previous = b;

            a.Next = rest;
            if (rest != null)
                rest.Previous = a;
            else
                last = a;

            a = rest; // Zum naechsten Paar weitergehen.
        }
    }

    // 9: Ganze Kette uebernehmen. Keine Figuren kopieren.
    public void Anhaengen(DoppeltVerketteteFigurListe andere)
    {
        if (andere == null || andere == this || andere.Count == 0) return;

        if (first == null)
            first = andere.first;
        else
        {
            last.Next = andere.first;
            andere.first.Previous = last;
        }

        last = andere.last;
        Count += andere.Count;

        andere.first = null;
        andere.last = null;
        andere.Count = 0;
    }
}

class Program
{
    static void Main()
    {
        DoppeltVerketteteFigurListe liste = new DoppeltVerketteteFigurListe();

        liste.Add(new Kugel("K1", 10));
        Zeige("Add(K1)", liste);
        liste.Add(new Wuerfel("W1", 3));
        Zeige("Add(W1)", liste);
        liste.Add(new Kugel("K2", 8));
        Zeige("Add(K2)", liste);
        liste.Add(new Wuerfel("W2", 4));
        Zeige("Add(W2)", liste);
        liste.Add(new Kugel("K3", 5));
        Zeige("Add(K3)", liste);

        liste.Push(new Wuerfel("W0-Push", 2));
        Zeige("Push", liste);
        Figur gepoppt = liste.Pop();
        Console.WriteLine("Pop liefert: " + gepoppt.Beschreibung);
        Zeige("Pop", liste);

        liste.InsertAt(2, new Kugel("K-Insert", 6));
        Zeige("InsertAt(2)", liste);
        liste.Remove(0);
        Zeige("Remove(0)", liste);
        liste.VertauscheBenachbarte();
        Zeige("VertauscheBenachbarte", liste);
        liste.Umdrehen();
        Zeige("Umdrehen", liste);

        DoppeltVerketteteFigurListe zweiteListe = new DoppeltVerketteteFigurListe();
        zweiteListe.Add(new Kugel("Z1", 3));
        Zeige("Zweite Liste: Add(Z1)", zweiteListe);
        zweiteListe.Add(new Wuerfel("Z2", 2));
        Zeige("Zweite Liste: Add(Z2)", zweiteListe);
        liste.Anhaengen(zweiteListe);
        Zeige("Anhaengen", liste);
        Zeige("Zweite Liste danach", zweiteListe);
    }

    static void Zeige(string schritt, DoppeltVerketteteFigurListe liste)
    {
        Console.WriteLine("\n--- " + schritt + " ---");
        Console.WriteLine("Vorwaerts:");
        liste.AusgabeVorwaerts();
        Console.WriteLine("Rueckwaerts:");
        liste.AusgabeRueckwaerts();
        Console.WriteLine("Count = " + liste.Count);
    }
}
