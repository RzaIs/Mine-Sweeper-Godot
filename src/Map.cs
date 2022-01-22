using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Map : Node2D
{
	[Signal] public delegate void GameStarted();
	[Signal] public delegate void GameOver();
	[Signal] public delegate void GameWon();
	[Signal] public delegate void FlagUsed(bool increase);

	public Board board;

	public TextureRect mapBG;

	public TileMap mineMap;
	public TileMap coverMap;
	public TileMap flagMap;

	public bool isGameStarted;
	public bool isGameActive;
	public bool isGameOver;

	public int selectedLevel;
	public int numberOfFlags;

	public override void _Ready()
	{
		mapBG = GetNode<TextureRect>("MapBG");
		mineMap = GetNode<TileMap>("MinesAndValues");
		coverMap = GetNode<TileMap>("CoverMap");
		flagMap = GetNode<TileMap>("FlagMap");
		isGameActive = false;
	}

	public void OnUILevelSelected(int selectedLevel)
	{
		Visible = true;
		this.selectedLevel = selectedLevel;

		if (selectedLevel == 0)
		{
			Init(8, 8, 10);
			numberOfFlags = 10;
			mapBG.RectSize = new Vector2(64, 64) * 8;
			Position = new Vector2(768, 448);
		}
		else if (selectedLevel == 1)
		{
			Init(16, 16, 40);
			numberOfFlags = 40;
			mapBG.RectSize = new Vector2(64, 64) * 16;
			Position = new Vector2(512, 192);
		}
		else if (selectedLevel == 2)
		{
			Init(16, 30, 99);
			numberOfFlags = 99;
			mapBG.RectSize = new Vector2(64 * 30, 64 * 16);
			Position = new Vector2(64, 192);
		}
	}

	public void RestartLevel()
	{
		ClearMaps();
		OnUILevelSelected(selectedLevel);
	}

	public void Init(int rows, int cols, int mineCount)
	{
		board = new Board(rows, cols, mineCount);
		PrintBoard();
		isGameActive = true;
		isGameOver = false;
		isGameStarted = false;
	}

	public void OnUIBackToMenu()
	{
		Visible = false;
		isGameActive = false;
		ClearMaps();
	}

	public void ClearMaps()
	{
		for (int i = 0; i < board.rows; i++)
		{
			for (int j = 0; j < board.cols; j++)
			{
				mineMap.SetCell(i, j, -1);
				coverMap.SetCell(i, j, -1);
				flagMap.SetCell(i, j, -1);
			}
		}
	}

	public override void _Process(float delta)
	{
		if ((Input.IsActionJustPressed("lclick") || Input.IsActionJustPressed("rclick")) && !isGameOver && isGameActive)
		{
			Vector2 mouseGrid = ToGrid(GetLocalMousePosition());

			if (Input.IsActionJustPressed("lclick"))
			{
				DoLeftClick(mouseGrid);
			}
			else if (Input.IsActionJustPressed("rclick"))
			{
				DoRightClick(mouseGrid);
			}
			isGameActive = !IsGameEnded();
			RefreshBoard();
		}
	}

	public void DoLeftClick(Vector2 mouseGrid)
	{
		if (flagMap.GetCellv(mouseGrid) == -1 && board.IsSelectable((int)mouseGrid.x, (int)mouseGrid.y))
		{
			board.Select((int)mouseGrid.x, (int)mouseGrid.y);
			if(!isGameStarted)
			{
				EmitSignal(nameof(GameStarted));
				isGameStarted = true;
			}
		}
	}

	public void DoRightClick(Vector2 mouseGrid)
	{
		if (coverMap.GetCellv(mouseGrid) != -1)
		{
			if (flagMap.GetCellv(mouseGrid) == -1 && numberOfFlags > 0)
			{
				flagMap.SetCellv(mouseGrid, 0);
				EmitSignal(nameof(FlagUsed), false);
				numberOfFlags--;
				board.Table[(int)mouseGrid.x, (int)mouseGrid.y].IsFlagged = true;
			}
			else if (flagMap.GetCellv(mouseGrid) == 0)
			{
				flagMap.SetCellv(mouseGrid, 2);
				EmitSignal(nameof(FlagUsed), true);
				numberOfFlags++;
			}
			else if (flagMap.GetCellv(mouseGrid) == 2)
			{
				flagMap.SetCellv(mouseGrid, -1);
				board.Table[(int)mouseGrid.x, (int)mouseGrid.y].IsFlagged = false;
			}

			if (!isGameStarted)
			{
				EmitSignal(nameof(GameStarted));
				isGameStarted = true;
			}
		}
	}

	public bool IsGameEnded()
	{
		for (int i = 0; i < board.rows; i++)
		{
			for (int j = 0; j < board.cols; j++)
			{
				if (board.Table[i, j].Value != 9 && !board.Table[i, j].IsSelected)
					return false;
			}
		}
		RevealMap();
		return true;
	}

	public Vector2 ToGrid(Vector2 position)
	{
		position.x = (float)Math.Floor(position.x / 64);
		position.y = (float)Math.Floor(position.y / 64);
		return position;
	}

	public void PrintBoard()
	{
		for (int i = 0; i < board.rows; i++)
		{
			for (int j = 0; j < board.cols; j++)
			{
				coverMap.SetCell(i, j, 0);
				mineMap.SetCell(i, j, board.Table[i, j].Value);
			}
		}
	}

	public void RevealMap()
	{
		if (isGameOver)
		{
			for (int i = 0; i < board.rows; i++)
			{
				for (int j = 0; j < board.cols; j++)
				{
					if (board.Table[i, j].IsFlagged)
					{
						if (board.Table[i, j].Value == 9)
						{
							board.Table[i, j].IsSelected = true;

							coverMap.SetCell(i, j, -1);

							if (flagMap.GetCell(i, j) == 0)
								mineMap.SetCell(i, j, 11);

							flagMap.SetCell(i, j, -1);
						}
						else
						{
							if (flagMap.GetCell(i, j) == 0)
								flagMap.SetCell(i, j, 1);
						}
					}
					else if (board.Table[i, j].Value == 9)
					{
						coverMap.SetCell(i, j, -1);
					}
				}
			}
		}
		else if(!isGameActive)
		{
			for (int i = 0; i < board.rows; i++)
			{
				for (int j = 0; j < board.cols; j++)
				{
					flagMap.SetCell(i, j, -1);
					if(board.Table[i, j].Value == 9)
					{
						mineMap.SetCell(i, j, 11);
						coverMap.SetCell(i, j, -1);
					}
				}
			}
			EmitSignal(nameof(GameWon));
		}
	}

	public void RefreshBoard()
	{
		for (int i = 0; i < board.rows; i++)
		{
			for (int j = 0; j < board.cols; j++)
			{
				if (board.Table[i, j].IsSelected)
				{
					coverMap.SetCell(i, j, -1);
					if (board.Table[i, j].Value == 9)
					{
						mineMap.SetCell(i, j, 10);
						isGameOver = true;
						EmitSignal(nameof(GameOver));
					}
				}
			}
		}
		RevealMap();
	}
}
