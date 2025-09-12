using DxfToolLib.Extensions;
using DxfToolLib.Models;
using Xunit;

namespace DxfToolLibTest.Extensions;

/// <summary>
/// Comprehensive test for FillFromPreviousRows with simple index-based completeness detection.
/// Complete rows have an index (first column filled), incomplete rows inherit from the last complete row.
/// </summary>
public class FillFromPreviousRowsComprehensiveTest
{
    [Fact]
    public void FillFromPreviousRows_ShouldInherit_FromLastCompleteRow()
    {
        // Arrange - Simple test with 5 columns using anonymous data
        var entries = new SimpleTestModel[]
        {
            // Complete row #1 (has Index)
            new() { Index = "1", ColumnA = "Alpha", ColumnB = "Beta", ColumnC = "Gamma", ColumnD = "Delta" },
            
            // Incomplete row - inherits from row #1, modifies only one column
            new() { Index = "", ColumnA = "", ColumnB = "Modified", ColumnC = "", ColumnD = "" },
            
            // Another incomplete row - inherits from row #1, modifies different column
            new() { Index = "", ColumnA = "", ColumnB = "", ColumnC = "Changed", ColumnD = "" },
            
            // Complete row #2 (new complete row)
            new() { Index = "2", ColumnA = "Apple", ColumnB = "Banana", ColumnC = "Cherry", ColumnD = "Date" },
            
            // Incomplete row - inherits from row #2
            new() { Index = "", ColumnA = "Updated", ColumnB = "", ColumnC = "", ColumnD = "" },
            
            // Final incomplete row - also inherits from row #2
            new() { Index = "", ColumnA = "", ColumnB = "", ColumnC = "", ColumnD = "Final" }
        };

        // Act
        var filled = entries.FillFromPreviousRows();

        // Assert
        Assert.Equal(6, filled.Length);
        
        // Row 1: Complete row should remain unchanged
        Assert.Equal("1", filled[0].Index);
        Assert.Equal("Alpha", filled[0].ColumnA);
        Assert.Equal("Beta", filled[0].ColumnB);
        Assert.Equal("Gamma", filled[0].ColumnC);
        Assert.Equal("Delta", filled[0].ColumnD);
        
        // Row 2: Inherits from row 1, only ColumnB is modified
        Assert.Equal("Alpha", filled[1].ColumnA);    // Inherited
        Assert.Equal("Modified", filled[1].ColumnB); // Original
        Assert.Equal("Gamma", filled[1].ColumnC);    // Inherited
        Assert.Equal("Delta", filled[1].ColumnD);    // Inherited
        
        // Row 3: Also inherits from row 1, only ColumnC is modified
        Assert.Equal("Alpha", filled[2].ColumnA);   // Inherited
        Assert.Equal("Beta", filled[2].ColumnB);    // Inherited
        Assert.Equal("Changed", filled[2].ColumnC); // Original
        Assert.Equal("Delta", filled[2].ColumnD);   // Inherited
        
        // Row 4: New complete row, should remain unchanged
        Assert.Equal("2", filled[3].Index);
        Assert.Equal("Apple", filled[3].ColumnA);
        Assert.Equal("Banana", filled[3].ColumnB);
        Assert.Equal("Cherry", filled[3].ColumnC);
        Assert.Equal("Date", filled[3].ColumnD);
        
        // Row 5: Inherits from row 4, only ColumnA is modified
        Assert.Equal("Updated", filled[4].ColumnA); // Original
        Assert.Equal("Banana", filled[4].ColumnB);  // Inherited from row 4
        Assert.Equal("Cherry", filled[4].ColumnC);  // Inherited from row 4
        Assert.Equal("Date", filled[4].ColumnD);    // Inherited from row 4
        
        // Row 6: Also inherits from row 4, only ColumnD is modified
        Assert.Equal("Apple", filled[5].ColumnA);  // Inherited from row 4
        Assert.Equal("Banana", filled[5].ColumnB); // Inherited from row 4
        Assert.Equal("Cherry", filled[5].ColumnC); // Inherited from row 4
        Assert.Equal("Final", filled[5].ColumnD);  // Original
    }

    [Fact]
    public void FillFromPreviousRows_ShouldHandle_EdgeCases()
    {
        // Arrange
        var entries = new SimpleTestModel[]
        {
            // No complete rows at the beginning
            new() { Index = "", ColumnA = "Orphan", ColumnB = "", ColumnC = "", ColumnD = "" },
            
            // First complete row
            new() { Index = "1", ColumnA = "Base", ColumnB = "Data", ColumnC = "Set", ColumnD = "One" },
            
            // Empty incomplete row
            new() { Index = "", ColumnA = "", ColumnB = "", ColumnC = "", ColumnD = "" },
            
            // All columns filled but no index (incomplete)
            new() { Index = "", ColumnA = "Full", ColumnB = "Row", ColumnC = "No", ColumnD = "Index" }
        };

        // Act
        var filled = entries.FillFromPreviousRows();

        // Assert
        Assert.Equal(4, filled.Length);
        
        // First row: No inheritance source, remains as-is
        Assert.Equal("Orphan", filled[0].ColumnA);
        Assert.Equal("", filled[0].ColumnB);
        
        // Second row: Complete, remains unchanged
        Assert.Equal("1", filled[1].Index);
        Assert.Equal("Base", filled[1].ColumnA);
        
        // Third row: Inherits everything from second row
        Assert.Equal("Base", filled[2].ColumnA);  // Inherited
        Assert.Equal("Data", filled[2].ColumnB);  // Inherited
        Assert.Equal("Set", filled[2].ColumnC);   // Inherited
        Assert.Equal("One", filled[2].ColumnD);   // Inherited
        
        // Fourth row: Inherits from second row but keeps original values
        Assert.Equal("Full", filled[3].ColumnA);  // Original (non-empty)
        Assert.Equal("Row", filled[3].ColumnB);   // Original (non-empty)
        Assert.Equal("No", filled[3].ColumnC);    // Original (non-empty)
        Assert.Equal("Index", filled[3].ColumnD); // Original (non-empty)
    }

    [Fact]
    public void FillFromPreviousRows_ShouldWork_WithSimpleIndexBasedRows()
    {
        // Arrange - Simple 5-column model where Index column indicates complete rows
        var entries = new SimpleTestModel[]
        {
            // Complete row (has Index)
            new() { Index = "1", ColumnA = "DataA1", ColumnB = "DataB1", ColumnC = "DataC1", ColumnD = "DataD1" },
            
            // Incomplete row (no Index, only one column changes)
            new() { Index = "", ColumnA = "", ColumnB = "ModifiedB", ColumnC = "", ColumnD = "" },
            
            // Another complete row
            new() { Index = "2", ColumnA = "DataA2", ColumnB = "DataB2", ColumnC = "DataC2", ColumnD = "DataD2" },
            
            // Multiple incomplete rows
            new() { Index = "", ColumnA = "", ColumnB = "", ColumnC = "ModifiedC", ColumnD = "" },
            new() { Index = "", ColumnA = "ModifiedA", ColumnB = "", ColumnC = "", ColumnD = "" }
        };

        // Act
        var filled = entries.FillFromPreviousRows();

        // Assert
        Assert.Equal(5, filled.Length);
        
        // First row unchanged (complete)
        Assert.Equal("1", filled[0].Index);
        Assert.Equal("DataA1", filled[0].ColumnA);
        
        // Second row inherits from first, but keeps its ColumnB modification
        Assert.Equal("DataA1", filled[1].ColumnA); // Inherited
        Assert.Equal("ModifiedB", filled[1].ColumnB); // Original
        Assert.Equal("DataC1", filled[1].ColumnC); // Inherited
        Assert.Equal("DataD1", filled[1].ColumnD); // Inherited
        
        // Third row unchanged (new complete row)
        Assert.Equal("2", filled[2].Index);
        Assert.Equal("DataA2", filled[2].ColumnA);
        
        // Fourth row inherits from third
        Assert.Equal("DataA2", filled[3].ColumnA); // Inherited from row 3
        Assert.Equal("DataB2", filled[3].ColumnB); // Inherited from row 3
        Assert.Equal("ModifiedC", filled[3].ColumnC); // Original
        Assert.Equal("DataD2", filled[3].ColumnD); // Inherited from row 3
        
        // Fifth row also inherits from third
        Assert.Equal("ModifiedA", filled[4].ColumnA); // Original
        Assert.Equal("DataB2", filled[4].ColumnB); // Inherited from row 3
        Assert.Equal("DataC2", filled[4].ColumnC); // Inherited from row 3
        Assert.Equal("DataD2", filled[4].ColumnD); // Inherited from row 3
    }

    // Simple test model with 5 columns - Index indicates complete row
    public class SimpleTestModel
    {
        public string Index { get; set; } = ""; // First column - if filled, row is complete
        public string ColumnA { get; set; } = "";
        public string ColumnB { get; set; } = "";
        public string ColumnC { get; set; } = "";
        public string ColumnD { get; set; } = "";
    }
}
