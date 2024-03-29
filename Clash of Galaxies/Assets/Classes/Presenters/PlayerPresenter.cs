﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Models;
using UnityEngine;
using Assets.Views;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Assets.Presenters
{
    class PlayerPresenter : IUnsubscribing
    {
        private Player player;
        private IPlayerView playerView;

        public PlayerPresenter(Player player, IPlayerView playerView)
        {
            this.player = player;
            this.playerView = playerView;

            List<ICardView> cardViews = CreateCardViews(player.CardsInHand);
            playerView.SetCardViews(cardViews);
            playerView.SetName(player.Name);

            playerView.DropCardCallback = DropCardView;
            playerView.PlayCurrentCardCallback = PlayCurrentCardView;
            playerView.EndMakeMoveCallback = ComleteMove;

            var collectionCards = (INotifyCollectionChanged)player.CardsInHand;
            collectionCards.CollectionChanged += CollectionCards_CollectionChanged;
        }

        private void CollectionCards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add) 
            {
                var cards = e.NewItems.Cast<Card>();
                List<ICardView> cardViews = CreateCardViews(cards);
                playerView.SetCardViews(cardViews);
            }
        }

        private List<ICardView> CreateCardViews(IEnumerable<Card> cards) 
        {
            List<ICardView> cardViews = new List<ICardView>();
            foreach (var card in cards)
            {
                var presenter = CardPresenterFactory.GetInstance().FindPresenter(card);

                if (presenter != null)
                    cardViews.Add(presenter.CardView);
                else 
                {
                    var view = CardViewFactory.GetInstance().GetNewView();
                    CardPresenterFactory.GetInstance().GetOrCreatePresenter(card, view);
                    cardViews.Add(view);
                }
                    
            }
            return cardViews;
        }

        public void ComleteMove() 
        {
            player.CompleteMove();
        }

        public void DropCardView(ICardView cardView)
        {
            CardPresenter cardPresenter = CardPresenterFactory.GetInstance().FindPresenter(cardView);
            if (cardPresenter != null) 
            {
                Card card = cardPresenter.Card;
                player.OnMakeMove(card);
            }
        }

        public void PlayCurrentCardView(ICardView cardView) 
        {
            CardPresenter cardPresenter = CardPresenterFactory.GetInstance().FindPresenter(cardView);
            if (cardPresenter != null)
            {
                Card card = cardPresenter.Card;
                player.OnPlayCurrentCard(card);
            }
        }

        public void Unsubscribe()
        {
            playerView.DropCardCallback = null;
            playerView.PlayCurrentCardCallback = null;
            playerView.EndMakeMoveCallback = null;

            var collectionCards = (INotifyCollectionChanged)player.CardsInHand;
            collectionCards.CollectionChanged -= CollectionCards_CollectionChanged;
        }
    }
}
