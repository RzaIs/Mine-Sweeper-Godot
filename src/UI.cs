using Godot;
using System;

public class UI : Control
{
	[Signal] public delegate void LevelSelected(int selectedLevel);
	[Signal] public delegate void RestartLevel();
	[Signal] public delegate void BackToMenu();

	public Control levelMenu;
	public Control inGameMenu;

	public Button smileButton;

	public Timer timer;

	public Label flagCount;
	public Label timerHour;
	public Label timerMinute;
	public Label timerSecond;

	public char selectedLevel;

	public int numberOfFlag;
	public int hours = 0;
	public int minutes = 0;
	public int seconds = 0;

	public override void _Ready()
	{
		levelMenu = GetNode<Control>("LevelSelectMenu");
		inGameMenu = GetNode<Control>("InGameMenu");
		smileButton = inGameMenu.GetNode<Button>("Smile");
		timer = inGameMenu.GetNode<Timer>("Timer");
		flagCount = inGameMenu.GetNode<Label>("FlagAmmount");
		timerHour = inGameMenu.GetNode("TimerLabels").GetNode<Label>("Hours");
		timerMinute = inGameMenu.GetNode("TimerLabels").GetNode<Label>("Minutes");
		timerSecond = inGameMenu.GetNode("TimerLabels").GetNode<Label>("Seconds");

	}

	public void OnEasyPressed()
	{
		selectedLevel = 'e';
		EmitSignal(nameof(LevelSelected), 0);
		numberOfFlag = 10;
		flagCount.Text = numberOfFlag.ToString();
		inGameMenu.Visible = true;
		levelMenu.Visible = false;
	}

	public void OnMediumPressed()
	{
		selectedLevel = 'm';
		EmitSignal(nameof(LevelSelected), 1);
		numberOfFlag = 40;
		flagCount.Text = numberOfFlag.ToString();
		inGameMenu.Visible = true;
		levelMenu.Visible = false;
	}

	public void OnHardPressed()
	{
		selectedLevel = 'h';
		EmitSignal(nameof(LevelSelected), 2);
		numberOfFlag = 99;
		flagCount.Text = numberOfFlag.ToString();
		inGameMenu.Visible = true;
		levelMenu.Visible = false;
	}

	public void UpdateFlagCount(bool increase)
	{
		numberOfFlag += increase ? 1 : -1;
		flagCount.Text = numberOfFlag.ToString();
	}

	public void OnMapGameOver()
	{
		smileButton.Text = ": (";
		timer.Stop();
	}

	public void OnSmilePressed()
	{
		timer.Stop();
		smileButton.Text = ": |";
		EmitSignal(nameof(RestartLevel));

		if (selectedLevel == 'e')
			numberOfFlag = 10;
		else if (selectedLevel == 'm')
			numberOfFlag = 40;
		else
			numberOfFlag = 99;
		flagCount.Text = numberOfFlag.ToString();

		seconds = 0; timerSecond.Text = "00";
		minutes = 0; timerMinute.Text = "00:";
		hours = 0; timerHour.Text = "00:";
	}

	public void OnMapGameWon()
	{
		smileButton.Text = ": )";
		timer.Stop();
	}

	public void OnMapGameStarted()
	{
		timer.Start();
	}

	public void OnTimerTimeout()
	{
		if(seconds < 59)
			seconds++;
		else
		{
			seconds = 0;
			if (minutes < 59)
				minutes++;
			else
				hours++;
		}

		if (seconds < 10)
			timerSecond.Text = "0" + seconds;
		else
			timerSecond.Text = seconds.ToString();

		if (minutes < 10)
			timerMinute.Text = "0" + minutes + ":";
		else
			timerMinute.Text = minutes + ":";

		if (hours < 10)
			timerHour.Text = "0" + hours + ":";
		else
			timerHour.Text = hours + ":";
	}

	public void OnLevelMenuButtonPressed()
	{
		OnSmilePressed();
		levelMenu.Visible = true;
		inGameMenu.Visible = false;
		EmitSignal(nameof(BackToMenu));
	}
}
