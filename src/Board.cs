using System;
using System.Collections.Generic;
using System.Linq;

public class Board
{
	public readonly int cols;
	public readonly int rows;
	public readonly int mineCount;

	private Cell[,] table;

	public Cell[,] Table { get => table; set => table = value; }

	public Board(int cols, int rows, int mineCount)
	{
		this.cols = cols;
		this.rows = rows;
		this.mineCount = mineCount;
		table = new Cell[rows, cols];
		CreateTable();
		FillMines();
		SetNumbers();
	}

	public void Select(int x, int y)
	{

		if (table[x, y].Value == 0)
			OpenEmptyCells(x, y);
		else
			table[x, y].IsSelected = true;
	}

	public bool IsSelectable(int x, int y)
	{
		if (IsInBoard(x, y))
			if (!table[x, y].IsSelected)
				return true;
		return false;
	}

	public void OpenEmptyCells(int x, int y)
	{
		table[x, y].IsSelected = true;

		Cell[] neighbors = GetNeighbors(x, y);

		foreach (Cell neighbor in neighbors)
		{
			if (!table[neighbor.Row, neighbor.Col].IsFlagged)
			{
				if (table[neighbor.Row, neighbor.Col].Value == 0 &&
				  !table[neighbor.Row, neighbor.Col].IsSelected)
				{
					OpenEmptyCells(neighbor.Row, neighbor.Col);
				}
				else
				{
					table[neighbor.Row, neighbor.Col].IsSelected = true;
				}
			}
		}
	}

	public void CreateTable()
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				table[i, j] = new Cell(i, j, 0);
			}
		}
	}

	public void FillMines()
	{
		Random rand = new Random();

		for (int i = 0; i < mineCount; i++)
		{
			bool mineLocated = false;

			while (!mineLocated)
			{
				int randR = rand.Next(rows);
				int randC = rand.Next(cols);

				if (table[randR, randC].Value == 0)
				{
					table[randR, randC].Value = 9;
					mineLocated = true;
				}

			}
		}
	}

	public void SetNumbers()
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				if (table[i, j].Value != 9)
				{
					int NumOfMines = CountMines(GetNeighbors(i, j));
					table[i, j].Value = NumOfMines;
				}
			}
		}
	}

	public int CountMines(Cell[] neighbors)
	{
		int amount = 0;

		foreach (Cell cell in neighbors)
		{
			if (cell.Value == 9)
				amount++;
		}
		return amount;
	}

	public Cell[] GetNeighbors(int x, int y)
	{
		LinkedList<Cell> neighbors = new LinkedList<Cell>();

		if (IsInBoard(x + 1, y + 1))
			neighbors.AddLast(table[x + 1, y + 1]);
		if (IsInBoard(x + 1, y - 1))
			neighbors.AddLast(table[x + 1, y - 1]);
		if (IsInBoard(x - 1, y + 1))
			neighbors.AddLast(table[x - 1, y + 1]);
		if (IsInBoard(x - 1, y - 1))
			neighbors.AddLast(table[x - 1, y - 1]);
		if (IsInBoard(x, y + 1))
			neighbors.AddLast(table[x, y + 1]);
		if (IsInBoard(x, y - 1))
			neighbors.AddLast(table[x, y - 1]);
		if (IsInBoard(x + 1, y))
			neighbors.AddLast(table[x + 1, y]);
		if (IsInBoard(x - 1, y))
			neighbors.AddLast(table[x - 1, y]);

		return neighbors.ToArray();
	}

	public bool IsInBoard(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < rows && y < cols)
			return true;
		return false;
	}
}
