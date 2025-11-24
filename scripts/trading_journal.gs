/**
 * Creates a "Trading Journal" Google Sheet with structured trade logging
 * and analytics. Run `createTradingJournal()` in Apps Script to generate
 * the sheet with validation-driven inputs and an auto-updating strategy pie chart.
 */
function createTradingJournal() {
  // Create spreadsheet and remove the default sheet to ensure clean setup
  const ss = SpreadsheetApp.create('Trading Journal');
  const defaultSheet = ss.getActiveSheet();
  defaultSheet.setName('Trades');
  const tradesSheet = defaultSheet;

  // Validation options sheet (hidden)
  const validationSheet = ss.insertSheet('Validation');
  validationSheet.getRange('A1').setValue('Strategies');
  validationSheet.getRange('A2:A7').setValues([
    ['Bullish SB60'],
    ['Bearish SB60'],
    ['Bullish Div'],
    ['Bearish Div'],
    ['Liquidity Sweep'],
    ['None'],
  ]);

  validationSheet.getRange('B1').setValue('Liquidity Levels');
  validationSheet.getRange('B2:B10').setValues([
    ['Last Week High'],
    ['Last Week Low'],
    ['Yesterday Value Area High'],
    ['Yesterday Value Area Low'],
    ['Yesterday High'],
    ['Yesterday Low'],
    ['Yesterday Volume Point of Control'],
    ['Settlement'],
    ['None'],
  ]);

  validationSheet.getRange('C1').setValue('Trade Slots');
  validationSheet.getRange('C2:C6').setValues([
    ['Trade 1'],
    ['Trade 2'],
    ['Trade 3'],
    ['Trade 4'],
    ['Trade 5'],
  ]);

  validationSheet.getRange('D1').setValue('Trade Results');
  validationSheet.getRange('D2:D3').setValues([
    ['🟢'],
    ['🔴'],
  ]);
  validationSheet.hideSheet();

  // Header row for trade entries
  tradesSheet.getRange('A1:E1').setValues([
    ['Date', 'Trade Slot', 'Strategy Used', 'Liquidity Level', 'Result'],
  ]);

  // Pre-size range to accommodate daily logging (rows for many days * 5 trades/day)
  const maxRows = 500; // enough for extended journaling
  tradesSheet.setMaxRows(maxRows);

  // Build validations
  const dateValidation = SpreadsheetApp.newDataValidation()
    .requireDate()
    .setAllowInvalid(false)
    .build();

  const tradeSlotValidation = SpreadsheetApp.newDataValidation()
    .requireValueInRange(validationSheet.getRange('C2:C6'), true)
    .setAllowInvalid(false)
    .build();

  const strategyValidation = SpreadsheetApp.newDataValidation()
    .requireValueInRange(validationSheet.getRange('A2:A7'), true)
    .setAllowInvalid(false)
    .build();

  const liquidityValidation = SpreadsheetApp.newDataValidation()
    .requireValueInRange(validationSheet.getRange('B2:B10'), true)
    .setAllowInvalid(false)
    .build();

  const tradeResultValidation = SpreadsheetApp.newDataValidation()
    .requireValueInRange(validationSheet.getRange('D2:D3'), true)
    .setAllowInvalid(false)
    .build();

  // Apply validations and some formatting
  tradesSheet.getRange(`A2:A${maxRows}`).setDataValidation(dateValidation);
  tradesSheet.getRange(`B2:B${maxRows}`).setDataValidation(tradeSlotValidation);
  tradesSheet.getRange(`C2:C${maxRows}`).setDataValidation(strategyValidation);
  tradesSheet.getRange(`D2:D${maxRows}`).setDataValidation(liquidityValidation);
  tradesSheet.getRange(`E2:E${maxRows}`).setDataValidation(tradeResultValidation);

  const resultRange = tradesSheet.getRange(`E2:E${maxRows}`);
  const resultRules = SpreadsheetApp.newConditionalFormatRule()
    .whenTextEqualTo('🟢')
    .setBackground('#b7e1cd')
    .setFontColor('#0b8043')
    .setRanges([resultRange])
    .build();

  const stopRules = SpreadsheetApp.newConditionalFormatRule()
    .whenTextEqualTo('🔴')
    .setBackground('#f4c7c3')
    .setFontColor('#c5221f')
    .setRanges([resultRange])
    .build();

  tradesSheet.setConditionalFormatRules([resultRules, stopRules]);

  tradesSheet.getRange('A1:E1').setFontWeight('bold');
  tradesSheet.setFrozenRows(1);
  tradesSheet.setColumnWidths(1, 5, 160);

  // Analytics sheet with live summary and pie chart
  const analyticsSheet = ss.insertSheet('Analytics');
  analyticsSheet.getRange('A1:B1').setValues([['Strategy', 'Trade Count']]);
  analyticsSheet.getRange('A2').setFormula(
    "=QUERY(Trades!C2:C, \"select Col1, count(Col1) where Col1 is not null group by Col1 label count(Col1) 'Trade Count'\", 0)"
  );

  const chart = analyticsSheet.newChart()
    .setChartType(Charts.ChartType.PIE)
    .addRange(analyticsSheet.getRange('A1:B50'))
    .setOption('title', 'Strategy Usage Distribution')
    .setOption('pieHole', 0)
    .setPosition(1, 4, 0, 0)
    .build();

  analyticsSheet.insertChart(chart);

  // Ensure the Trades sheet is the first visible sheet for quick entry
  ss.setActiveSheet(tradesSheet);

  Logger.log('Trading Journal created: %s', ss.getUrl());
}
