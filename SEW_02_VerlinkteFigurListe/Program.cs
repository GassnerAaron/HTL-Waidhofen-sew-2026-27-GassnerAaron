using System;
using System.IO;

interface IFigur
{
    double berechneOberflaeche();
    double berechneVolumen();
}

abstract class Figur : IFigur
{
    public Figur(string beschreibung)
    {
        Beschreibung = beschreibung;
    }

    public string Beschreibung { get; }

    // next-Referenz direkt in der Figur-Klasse (wie in der urspruenglichen Uebung).
    public Figur Next { get; set; }

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
        return 4.0 * Math.PI * Math.Pow(Radius, 2);
    }

    public override double berechneVolumen()
    {
        return (4.0 * Math.PI * Math.Pow(Radius, 3)) / 3.0;
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
        return 6.0 * Math.Pow(Seitenlaenge, 2);
    }

    public override double berechneVolumen()
    {
        return Math.Pow(Seitenlaenge, 3);
    }
}
class VerlinkteFigurListe
{
    private Figur first;
    public int Count { get; private set; }

    // Liefert die Figur am Index oder null, wenn der Index ungueltig ist.
    private Figur GetAt(int index)
    {
        if (index < 0 || index >= Count)
        {
            return null;
        }

        Figur current = first;
        for (int i = 0; i < index; i++)
        {
            current = current.Next;
        }
        return current;
    }

    // Haengt eine einzelne Figur hinten an; null wird ignoriert.
    public void Add(Figur fig)
    {
        if (fig == null)
        {
            return;
        }

        fig.Next = null;
        if (first == null)
        {
            first = fig;
        }
        else
        {
            Figur letzte = first;
            while (letzte.Next != null)
            {
                letzte = letzte.Next;
            }
            letzte.Next = fig;
        }
        Count++;
    }

    // Geht von first bis null und gibt jede Figur mit ihren Berechnungen aus.
    public void AusgabeAllerFiguren()
    {
        Figur current = first;
        while (current != null)
        {
            Console.WriteLine(current);
            current = current.Next;
        }
    }

    // Fuegt eine einzelne Figur vorne ein; zuerst den bisherigen Anfang sichern.
    public void Push(Figur fig)
    {
        if (fig == null)
        {
            return;
        }

        fig.Next = first;
        first = fig;
        Count++;
    }

    // Entfernt die erste Figur und trennt sie von der restlichen Liste.
    public Figur Pop()
    {
        if (first == null)
        {
            return null;
        }

        Figur ergebnis = first;
        first = first.Next;
        ergebnis.Next = null;
        Count--;
        return ergebnis;
    }

    // Fuegt vorne, hinten oder zwischen zwei Figuren ein, je nach Index.
    public void InsertAt(int index, Figur fig)
    {
        if (fig == null)
        {
            return;
        }
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

        Figur prev = GetAt(index - 1);
        fig.Next = prev.Next;
        prev.Next = fig;
        Count++;
    }

    // Entfernt eine Figur am Index; ungueltige Indizes veraendern nichts.
    public void Remove(int index)
    {
        if (index < 0 || index >= Count)
        {
            return;
        }
        if (index == 0)
        {
            Pop();
            return;
        }

        Figur prev = GetAt(index - 1);
        Figur entfernt = prev.Next;
        prev.Next = entfernt.Next;
        entfernt.Next = null;
        Count--;
    }

    // Dreht alle Next-Pfeile in einem Durchlauf um; Count bleibt gleich.
    public void Umdrehen()
    {
        Figur prev = null;
        Figur current = first;
        while (current != null)
        {
            Figur next = current.Next;
            current.Next = prev;
            prev = current;
            current = next;
        }
        first = prev;
    }

    // Vertauscht vollstaendige Paare; eine einzelne letzte Figur bleibt stehen.
    public void VertauscheBenachbarte()
    {
        Figur prevPaarEnde = null;
        Figur current = first;
        while (current != null && current.Next != null)
        {
            Figur a = current;
            Figur b = a.Next;
            Figur rest = b.Next;

            b.Next = a;
            a.Next = rest;
            if (prevPaarEnde == null)
            {
                first = b;
            }
            else
            {
                prevPaarEnde.Next = b;
            }
            prevPaarEnde = a;
            current = rest;
        }
    }

    // Uebernimmt die Kette der anderen Liste und leert danach deren Verwaltung.
    public void Anhaengen(VerlinkteFigurListe andere)
    {
        if (andere == null || andere == this || andere.Count == 0)
        {
            return;
        }
        if (first == null)
        {
            first = andere.first;
        }
        else
        {
            Figur letzte = first;
            while (letzte.Next != null)
            {
                letzte = letzte.Next;
            }
            letzte.Next = andere.first;
        }
        Count += andere.Count;
        andere.first = null;
        andere.Count = 0;
    }
}

class Program
{
    private static int tests;
    private static int fehler;

    // Fuehrt den vorgegebenen Ablauf und danach die Randfalltests aus.
    static void Main()
    {
        VerlinkteFigurListe liste = new VerlinkteFigurListe();
        Kugel k1 = new Kugel("K1", 10);
        Wuerfel w1 = new Wuerfel("W1", 3);
        Kugel k2 = new Kugel("K2", 8);
        Wuerfel w2 = new Wuerfel("W2", 4);
        Kugel k3 = new Kugel("K3", 5);

        liste.Add(k1);
        ZeigeSchritt("Add(K1)", liste, "K1", 1);
        liste.Add(w1);
        ZeigeSchritt("Add(W1)", liste, "K1 W1", 2);
        liste.Add(k2);
        ZeigeSchritt("Add(K2)", liste, "K1 W1 K2", 3);
        liste.Add(w2);
        ZeigeSchritt("Add(W2)", liste, "K1 W1 K2 W2", 4);
        liste.Add(k3);
        ZeigeSchritt("Add x5", liste, "K1 W1 K2 W2 K3", 5);

        Wuerfel w0 = new Wuerfel("W0-Push", 2);
        liste.Push(w0);
        ZeigeSchritt("Push(W0-Push)", liste, "W0-Push K1 W1 K2 W2 K3", 6);
        Figur gepoppt = liste.Pop();
        ZeigeSchritt("Pop() liefert " + gepoppt.Beschreibung, liste, "K1 W1 K2 W2 K3", 5);
        Pruefe("Pop liefert dieselbe Figur mit Next == null", gepoppt == w0 && gepoppt.Next == null);

        Kugel eingefuegt = new Kugel("K-Insert", 6);
        liste.InsertAt(2, eingefuegt);
        ZeigeSchritt("InsertAt(2, K-Insert)", liste, "K1 W1 K-Insert K2 W2 K3", 6);
        liste.Remove(0);
        ZeigeSchritt("Remove(0)", liste, "W1 K-Insert K2 W2 K3", 5);
        liste.VertauscheBenachbarte();
        ZeigeSchritt("VertauscheBenachbarte()", liste, "K-Insert W1 W2 K2 K3", 5);
        Pruefe("Paartausch behaelt dieselben Figuren", eingefuegt.Next == w1 && w1.Next == w2 && w2.Next == k2 && k2.Next == k3 && k3.Next == null);
        liste.Umdrehen();
        ZeigeSchritt("Umdrehen()", liste, "K3 K2 W2 W1 K-Insert", 5);
        Pruefe("Umdrehen behaelt dieselben Figuren", k3.Next == k2 && k2.Next == w2 && w2.Next == w1 && w1.Next == eingefuegt && eingefuegt.Next == null);

        VerlinkteFigurListe zweiteListe = new VerlinkteFigurListe();
        Kugel z1 = new Kugel("Z1", 3);
        Wuerfel z2 = new Wuerfel("Z2", 2);
        zweiteListe.Add(z1);
        ZeigeSchritt("Zweite Liste: Add(Z1)", zweiteListe, "Z1", 1);
        zweiteListe.Add(z2);
        ZeigeSchritt("Zweite Liste: Add(Z2)", zweiteListe, "Z1 Z2", 2);
        liste.Anhaengen(zweiteListe);
        ZeigeSchritt("Anhaengen(zweiteListe)", liste, "K3 K2 W2 W1 K-Insert Z1 Z2", 7);
        Console.WriteLine("zweiteListe.Count = " + zweiteListe.Count);
        Pruefe("Zweite Liste ist leer; Figuren wurden uebernommen", StimmtListe(zweiteListe, "", 0) && eingefuegt.Next == z1 && z1.Next == z2 && z2.Next == null);

        TesteRandfaelle();
        Console.WriteLine();
        Console.WriteLine("Tests: " + tests + ", OK: " + (tests - fehler) + ", FEHLER: " + fehler);
        Environment.ExitCode = fehler == 0 ? 0 : 1;
    }

    // Zeigt nach einem Schritt die gesamte Liste, Count und den Soll-Ist-Test.
    private static void ZeigeSchritt(string titel, VerlinkteFigurListe liste, string erwartet, int count)
    {
        Console.WriteLine();
        Console.WriteLine("=== " + titel + " ===");
        liste.AusgabeAllerFiguren();
        Console.WriteLine("Count = " + liste.Count);
        Pruefe(titel + ": Reihenfolge und Count", StimmtListe(liste, erwartet, count));
    }

    // Vergleicht die echte Ausgabe mit der erwarteten Reihenfolge, ohne Arrays.
    private static bool StimmtListe(VerlinkteFigurListe liste, string erwartet, int count)
    {
        TextWriter vorher = Console.Out;
        using (StringWriter ausgabe = new StringWriter())
        {
            try
            {
                Console.SetOut(ausgabe);
                liste.AusgabeAllerFiguren();
            }
            finally
            {
                Console.SetOut(vorher);
            }

            string reihenfolge = "";
            int anzahl = 0;
            using (StringReader reader = new StringReader(ausgabe.ToString()))
            {
                string zeile = reader.ReadLine();
                while (zeile != null)
                {
                    int ende = zeile.IndexOf(" (Oberflaeche=", StringComparison.Ordinal);
                    if (ende < 0)
                    {
                        return false;
                    }
                    if (anzahl > 0)
                    {
                        reihenfolge += " ";
                    }
                    reihenfolge += zeile.Substring(0, ende);
                    anzahl++;
                    zeile = reader.ReadLine();
                }
            }
            return reihenfolge == erwartet && anzahl == count && liste.Count == count;
        }
    }

    // Gibt fuer jeden Test OK oder FEHLER aus und zaehlt die Ergebnisse.
    private static void Pruefe(string name, bool richtig)
    {
        tests++;
        if (!richtig)
        {
            fehler++;
        }
        Console.WriteLine((richtig ? "OK: " : "FEHLER: ") + name);
    }

    // Prueft leere und kurze Listen, Indexgrenzen und das Verbinden von Listen.
    private static void TesteRandfaelle()
    {
        Console.WriteLine();
        Console.WriteLine("=== Randfalltests ===");
        VerlinkteFigurListe leer = new VerlinkteFigurListe();
        Pruefe("Leere Liste: Pop gibt null", leer.Pop() == null && leer.Count == 0);
        leer.Remove(0);
        leer.Umdrehen();
        leer.VertauscheBenachbarte();
        leer.AusgabeAllerFiguren();
        Pruefe("Leere Liste: Remove, Umdrehen, Paartausch und Ausgabe", StimmtListe(leer, "", 0));

        Wuerfel a = new Wuerfel("A", 1);
        leer.Add(a);
        leer.Umdrehen();
        Pruefe("Ein Element: Umdrehen", StimmtListe(leer, "A", 1) && a.Next == null);
        leer.VertauscheBenachbarte();
        Pruefe("Ein Element: Paartausch", StimmtListe(leer, "A", 1) && a.Next == null);
        Figur entfernt = leer.Pop();
        Pruefe("Ein Element: Pop", entfernt == a && entfernt.Next == null && StimmtListe(leer, "", 0));
        leer.Add(a);
        leer.Remove(0);
        Pruefe("Ein Element: Remove(0)", StimmtListe(leer, "", 0) && a.Next == null);

        VerlinkteFigurListe indizes = new VerlinkteFigurListe();
        indizes.Add(a);
        indizes.Remove(-1);
        Pruefe("Remove(-1) aendert nichts", StimmtListe(indizes, "A", 1));
        indizes.Remove(indizes.Count);
        Pruefe("Remove(Count) aendert nichts", StimmtListe(indizes, "A", 1));
        Wuerfel x = new Wuerfel("X", 1);
        indizes.InsertAt(-5, x);
        Pruefe("InsertAt(-5) fuegt vorne ein", StimmtListe(indizes, "X A", 2) && x.Next == a);
        Wuerfel y = new Wuerfel("Y", 1);
        indizes.InsertAt(99, y);
        Pruefe("InsertAt(99) fuegt hinten ein", StimmtListe(indizes, "X A Y", 3) && a.Next == y && y.Next == null);
        indizes.Remove(1);
        Pruefe("Remove in der Mitte trennt die entfernte Figur", StimmtListe(indizes, "X Y", 2) && x.Next == y && a.Next == null);
        indizes.Remove(1);
        Pruefe("Remove am Ende", StimmtListe(indizes, "X", 1) && x.Next == null);

        indizes.Anhaengen(leer);
        Pruefe("Anhaengen einer leeren Liste", StimmtListe(indizes, "X", 1) && StimmtListe(leer, "", 0));
        indizes.Anhaengen(null);
        Pruefe("Anhaengen mit null", StimmtListe(indizes, "X", 1));
        leer.Anhaengen(indizes);
        Pruefe("Anhaengen an eine leere eigene Liste", StimmtListe(leer, "X", 1) && StimmtListe(indizes, "", 0) && leer.Pop() == x);

        VerlinkteFigurListe paare = new VerlinkteFigurListe();
        Wuerfel b = new Wuerfel("B", 1);
        Wuerfel c = new Wuerfel("C", 1);
        Wuerfel d = new Wuerfel("D", 1);
        paare.Add(a);
        paare.Add(b);
        paare.Add(c);
        paare.Add(d);
        paare.VertauscheBenachbarte();
        Pruefe("Gerade Anzahl: A B C D wird B A D C", StimmtListe(paare, "B A D C", 4) && b.Next == a && a.Next == d && d.Next == c && c.Next == null);
        paare.VertauscheBenachbarte();
        Pruefe("Zweimal Paartausch ergibt die Ausgangsliste", StimmtListe(paare, "A B C D", 4));
        paare.Umdrehen();
        paare.Umdrehen();
        Pruefe("Zweimal Umdrehen ergibt die Ausgangsliste", StimmtListe(paare, "A B C D", 4));
        paare.Anhaengen(paare);
        Pruefe("Selbst-Anhaengen wird ignoriert", StimmtListe(paare, "A B C D", 4));
        paare.Add(null);
        paare.Push(null);
        paare.InsertAt(1, null);
        Pruefe("Null-Figuren werden ignoriert", StimmtListe(paare, "A B C D", 4));

        VerlinkteFigurListe grenzen = new VerlinkteFigurListe();
        Wuerfel e = new Wuerfel("E", 1);
        Wuerfel f = new Wuerfel("F", 1);
        grenzen.InsertAt(0, e);
        grenzen.InsertAt(grenzen.Count, f);
        Pruefe("InsertAt(0) in leerer Liste und InsertAt(Count)", StimmtListe(grenzen, "E F", 2) && e.Next == f);
        Pruefe("Berechnungen von Kugel und Wuerfel", Math.Abs(new Kugel("Testkugel", 1).berechneOberflaeche() - 4 * Math.PI) < 0.000001 && Math.Abs(new Kugel("Testkugel", 1).berechneVolumen() - 4 * Math.PI / 3) < 0.000001 && new Wuerfel("Testwuerfel", 2).berechneOberflaeche() == 24 && new Wuerfel("Testwuerfel", 2).berechneVolumen() == 8);
    }
}
