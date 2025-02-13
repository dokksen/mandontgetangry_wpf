﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ManDontGetAngryGame.Enums;
using ManDontGetAngryGame.Events;
using ManDontGetAngryGame.Game;
using ManDontGetAngryGame.Model;

namespace ManDontGetAngryGame.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {



        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<CellStatusViewModel> _cells = new ObservableCollection<CellStatusViewModel>();
        public ObservableCollection<CellStatusViewModel> Cells
        {
            get { return _cells; }
        }

        private void SetupCellStatusViewModels(int rows, int cols)
        {
            CellPositions.SetupPositions(rows, cols);
            foreach (var item in CellPositions.Positions)
            {
                _cells.Add(new CellStatusViewModel(item.Value.Item1, item.Value.Item2,item.Key.Item1, item.Key.Item2, item.Key.Item3));
            }

        }

        private GameLogic _gameLogic;
        public MainViewModel(GameLogic gameLogic)
        {
            _gameLogic = gameLogic;
            SetupCellStatusViewModels(_gameLogic.NumRows, _gameLogic.NumCols);
            _gameLogic.CellStatusChanged += _gameBoard_CellStatusChanged;
            _gameLogic.GameFinished += _gameLogic_GameFinished;
            _gameLogic.StartGame();
        }

        private void _gameBoard_CellStatusChanged(object sender, CellStatusChangedEventArgs e)
        {
            fetchCell(e.Identifier).SetPiece(e.NewPiece);
        }


        private void _gameLogic_GameFinished(object sender, GameFinishedEventArgs e)
        {
            Winner = e.Winner;
            IsGameFinished = true;
            Task.Delay(4000).ContinueWith((x) => { IsGameFinished = false; });
        }

        private CellStatusViewModel fetchCell(CellId identifier)
        {
            return Cells.First(cell => cell.Identifier.Equals(identifier));
        }

        private EActivePlayer _activePlayer = EActivePlayer.Blue;
        public EActivePlayer ActivePlayer
        {
            get { return _activePlayer; }
            set
            {
                _activePlayer = value;
                OnPropertyChanged(nameof(ActivePlayer));
            }
        }

        private EActivePlayer _winner;
        public EActivePlayer Winner
        {
            get { return _winner; }
            set
            {
                if (_winner != value)
                {
                    _winner = value;
                    OnPropertyChanged(nameof(Winner));
                }
            }
        }

        private bool _isGameFinished = false;

        public bool IsGameFinished
        {
            get { return _isGameFinished; }
            set
            {
                if (_isGameFinished != value)
                {
                    _isGameFinished = value;
                    OnPropertyChanged(nameof(IsGameFinished));
                }
            }
        }

        private int _dice = 0;
        public int Dice
        {
            get { return _dice; }
            set
            {
                _dice = value;
                OnPropertyChanged(nameof(Dice));
            }
        }

        private RelayCommand _throwDiceCommand;

        private Random random = new Random();

        public RelayCommand ThrowDiceCommand
        {
            get
            {
                return _throwDiceCommand ??
                       (_throwDiceCommand = new RelayCommand(
                           (x) =>
                           {
                               Dice = random.Next(6) + 1;
                               _gameLogic.MovePiece(Dice, ActivePlayer);
                               ActivePlayer = _gameLogic.ActivePlayer;
                           }, // Execute
                           (x) => { return true; } // CanExecute
                       ));
            }
        }

    }
}
