// -------------------------------------------------------------------------------------------------
//
//    FVG Highlighter - Fair Value Gap Indicator for cTrader
//
//    This indicator detects and highlights Fair Value Gaps (FVG) on the chart.
//    A FVG occurs when there is an imbalance between buyers and sellers,
//    creating a gap between the low of candle N-1 and the high of candle N+1
//    (bullish FVG) or vice versa (bearish FVG).
//
//    Author: J. Cornier
//    Version: 1.0
//    Last Updated: 2026-01-17
//
// -------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;

[Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
public class FVGIndicator : Indicator
{
    [Parameter("FVG Color", DefaultValue = "Yellow")]
    public Color FVGColor { get; set; }
    
    [Parameter("Minimum Gap Pips", DefaultValue = 3)]
    public int MinimumGapPips { get; set; }
    
    [Parameter("Show FVG Rectangles", DefaultValue = true)]
    public bool ShowFVGRectangles { get; set; }

    [Parameter("Rectangle Opacity", DefaultValue = 50, MinValue = 0, MaxValue = 255)]
    public int RectangleOpacity { get; set; }

    [Parameter("Enabled", DefaultValue = true)]
    public bool Enabled { get; set; }

    private bool _wasEnabled = true;

    protected override void Initialize()
    {
        // S'abonner aux événements de nouvelles bougies
        Bars.BarOpened += OnBarOpened;
    }

    protected override void OnDestroy()
    {
        ClearAllFVG();
    }

    private void ClearAllFVG()
    {
        // Supprimer tous les rectangles FVG
        var objectNames = Chart.Objects;
        foreach (var obj in objectNames)
        {
            if (obj.Name.StartsWith("FVG_"))
            {
                Chart.RemoveObject(obj.Name);
            }
        }

        // Réinitialiser les couleurs des bougies
        for (int i = 0; i < Bars.Count; i++)
        {
            Chart.ResetBarColor(i);
        }
    }

    public override void Calculate(int index)
    {
        // Détecter le changement d'état Enabled
        if (_wasEnabled && !Enabled)
        {
            // L'utilisateur vient de désactiver - nettoyer le graphique
            ClearAllFVG();
            _wasEnabled = false;
            return;
        }
        else if (!_wasEnabled && Enabled)
        {
            // L'utilisateur vient de réactiver - redessiner tout
            _wasEnabled = true;
        }

        if (!Enabled)
            return;

        // Calculer sur toutes les bougies disponibles lors du chargement initial
        if (index < 3)
            return;

        CheckAndDrawFVG(index - 2); // Vérifier 2 bougies en arrière pour être sûr
    }

    private void OnBarOpened(BarOpenedEventArgs args)
    {
        // Quand une nouvelle bougie s'ouvre, vérifier s'il y a un FVG
        // On vérifie la bougie n-3 pour être sûr que le pattern est complet
        int centralIndex = Bars.Count - 3; // 3 bougies en arrière
        
        if (centralIndex >= 1)
        {
            CheckAndDrawFVG(centralIndex);
        }
    }

    private void CheckAndDrawFVG(int centralIndex)
    {
        if (centralIndex >= 1 && HasFVG(centralIndex))
        {
            // Dessiner le FVG
            Chart.SetBarFillColor(centralIndex, FVGColor);
            Chart.SetBarOutlineColor(centralIndex, FVGColor);
            
            if (ShowFVGRectangles)
            {
                DrawFVGRectangle(centralIndex);
            }
        }
    }

    private bool HasFVG(int centralIndex)
    {
        if (centralIndex < 1 || centralIndex >= Bars.Count - 1)
            return false;

        // Bougie précédente (n-1)
        double prevHigh = Bars.HighPrices[centralIndex - 1];
        double prevLow = Bars.LowPrices[centralIndex - 1];
        
        // Bougie suivante (n+1)
        double nextHigh = Bars.HighPrices[centralIndex + 1];
        double nextLow = Bars.LowPrices[centralIndex + 1];

        // Convertir l'écart minimum en unités de prix
        double minGapPrice = MinimumGapPips * Symbol.PipSize;

        // FVG Haussier
        bool bullishFVG = (prevLow - nextHigh) >= minGapPrice;

        // FVG Baissier
        bool bearishFVG = (nextLow - prevHigh) >= minGapPrice;

        return bullishFVG || bearishFVG;
    }
    
    private void DrawFVGRectangle(int centralIndex)
    {
        if (centralIndex < 1 || centralIndex >= Bars.Count - 1)
            return;

        double prevHigh = Bars.HighPrices[centralIndex - 1];
        double prevLow = Bars.LowPrices[centralIndex - 1];
        double nextHigh = Bars.HighPrices[centralIndex + 1];
        double nextLow = Bars.LowPrices[centralIndex + 1];

        double gapHigh, gapLow;
        
        if (prevLow > nextHigh) // FVG Haussier
        {
            gapHigh = prevLow;
            gapLow = nextHigh;
        }
        else if (prevHigh < nextLow) // FVG Baissier
        {
            gapHigh = nextLow;
            gapLow = prevHigh;
        }
        else
        {
            return;
        }

        DateTime startTime = Bars.OpenTimes[centralIndex - 1];
        DateTime endTime = Bars.OpenTimes[centralIndex + 1];
        
        var rectangle = Chart.DrawRectangle($"FVG_{centralIndex}", startTime, gapHigh, endTime, gapLow, FVGColor);
        rectangle.IsFilled = true;
        rectangle.Color = Color.FromArgb(RectangleOpacity, FVGColor);
    }
}