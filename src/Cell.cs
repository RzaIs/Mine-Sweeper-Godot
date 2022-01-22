public class Cell
{
	private int row;
	private int col;
	private int value;
	private bool isSelected;
	private bool isFlagged;

	public int Row { get => row; set => row = value; }
	public int Col { get => col; set => col = value; }
	public int Value { get => this.value; set => this.value = value; }
	public bool IsSelected { get => isSelected; set => isSelected = value; }
	public bool IsFlagged { get => isFlagged; set => isFlagged = value; }

	public Cell(int row, int col, int value)
	{
		this.row = row;
		this.col = col;
		this.value = value;
		isSelected = false;
		isFlagged = false;
	}
}
