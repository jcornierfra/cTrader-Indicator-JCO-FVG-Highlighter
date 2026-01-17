# FVG Highlighter - cTrader Indicator

Fair Value Gap (FVG) indicator for cTrader that detects and highlights price imbalances on charts.

## What is a Fair Value Gap?

A Fair Value Gap (FVG) occurs when there is an imbalance between buyers and sellers, creating a gap in price action:

- **Bullish FVG**: Gap between the low of candle N-1 and the high of candle N+1 (price moved up quickly)
- **Bearish FVG**: Gap between the high of candle N-1 and the low of candle N+1 (price moved down quickly)

These gaps often act as areas of interest where price may return to "fill" the imbalance.

## Features

- Automatic detection of bullish and bearish FVGs
- Visual highlighting with customizable color
- Optional rectangle overlay showing the gap zone
- Configurable minimum gap size (in pips)
- Adjustable rectangle opacity
- Enable/disable toggle with automatic cleanup

## Parameters

| Parameter | Description | Default |
|-----------|-------------|---------|
| FVG Color | Color for highlighting FVG candles and rectangles | Yellow |
| Minimum Gap Pips | Minimum gap size required to detect an FVG | 3 |
| Show FVG Rectangles | Display rectangle overlays on FVG zones | true |
| Rectangle Opacity | Transparency of rectangles (0-255) | 50 |
| Enabled | Enable/disable the indicator | true |

## Installation

1. Download the `FVG Highlighter.cs` file
2. In cTrader, go to **Automate** > **Indicators**
3. Click **New Indicator** and replace the code with the downloaded file
4. Build the indicator
5. Add it to your chart from the Indicators panel

## Usage

Once added to a chart, the indicator will automatically:
- Scan historical bars for FVG patterns
- Highlight detected FVGs by coloring the central candle
- Draw semi-transparent rectangles showing the gap zone (if enabled)
- Detect new FVGs in real-time as new candles form

## License

This project is provided as-is for educational and trading purposes.

## Author

J. Cornier - 2026
