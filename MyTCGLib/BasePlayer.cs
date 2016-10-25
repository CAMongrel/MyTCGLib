using System;
using System.Collections.Generic;

namespace MyTCGLib
{
   /// <summary>
   /// Runtime player class inside a match. 
   /// 
   /// Should not be used to persist information about a human player across multiple sessions.
   /// </summary>
   public class BasePlayer
   {
      /// <summary>
      /// The current deck of the player at runtime. Must be set by derived classes.
      /// </summary>
      public BaseDeck Deck { get; protected set; }

      /// <summary>
      /// The current hand of the player at runtime. Must be set by derived classes.
      /// </summary>
      /// <value>The hand.</value>
      public BaseHand Hand { get; protected set; }

      /// <summary>
      /// The name of the player at runtime. Should only be used for display purposes.
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// A reference to the currently running match or null if this player is not attached to a specific match.
      /// </summary>
      public BaseMatch ActiveMatch { get; set; }

      /// <summary>
      /// Gets or sets a value indicating whether this <see cref="T:MyTCGLib.BasePlayer"/> is locally controlled. 
      /// 
      /// Derived classes can set this to false, if this player is e.g. an AI controlled or network (remote) controlled player.
      /// 
      /// Default value is true.
      /// </summary>
      /// <value><c>true</c> if is locally controlled; otherwise, <c>false</c>.</value>
      public bool IsLocallyControlled { get; protected set; }

      public BasePlayer ()
      {
         ActiveMatch = null;
         Deck = null;
         Hand = null;
         Name = string.Empty;
         IsLocallyControlled = true;
      }

      #region CreateRandomDeck
      /// <summary>
      /// Creates a random deck from a set of available card types. The types in the collection must be derived from <see cref="T:MyTCGLib.BaseCard"/>,
      /// otherwise the method will create an exception.
      /// 
      /// The Deck property must be initialized before calling this method.
      /// </summary>
      /// <param name="availCardTypes">List of available card types.</param>
      /// <param name="maxCount">Number of card instances in the resulting deck. Must be equal or smaller to the number of elements in availCardTypes</param>
      /// <param name="rnd">Pseudo-RNG instance. The method will create its own instance if rnd is null.</param>
      public void CreateRandomDeckFromTypes(IEnumerable<Type> availCardTypes, int maxCount, Random rnd = null)
      {
         if (Deck == null)
         {
            throw new InvalidOperationException("Deck cannot be null");
         }

         List<BaseCard> availCards = new List<BaseCard>();
         foreach (Type t in availCardTypes)
         {
            BaseCard card = (BaseCard)Activator.CreateInstance(t);
            availCards.Add(card);
         }
         CreateRandomDeck(availCards, maxCount, rnd);
      }

      /// <summary>
      /// Creates a random deck from a set of available card instances. 
      /// 
      /// The Deck property must be initialized before calling this method.
      /// </summary>
      /// <param name="availCards">List of available card instances.</param>
      /// <param name="maxCount">Number of card instances in the resulting deck. Must be equal or smaller to the number of elements in availCardTypes</param>
      /// <param name="rnd">Pseudo-RNG instance. The method will create its own instance if rnd is null.</param>
      public void CreateRandomDeck(IEnumerable<BaseCard> availCards, int maxCount, Random rnd = null)
      {
         if (Deck == null)
         {
            throw new InvalidOperationException("Deck cannot be null");
         }

         if (rnd == null)
         {
            rnd = new Random();
         }

         List<BaseCard> remainingAvailCards = new List<BaseCard>(availCards);

         if (maxCount > remainingAvailCards.Count)
         {
            maxCount = remainingAvailCards.Count;
         }

         List<BaseCard> newDeck = new List<BaseCard>(maxCount);
         for (int i = 0; i < maxCount; i++)
         {
            int idx = rnd.Next(remainingAvailCards.Count);
            BaseCard newCard = remainingAvailCards[idx].CreateCopy();
            newCard.Owner = this;
            newDeck.Add(newCard);
            remainingAvailCards.RemoveAt(idx);
         }
         Deck.SetDeck(newDeck);
      }
      #endregion

      #region DrawCard
      /// <summary>
      /// Draws a card from the deck.
      /// </summary>
      /// <returns><c>true</c>, if card was drawn, <c>false</c> otherwise.</returns>
      public virtual bool DrawCard()
      {
         if (Hand == null)
         {
            throw new InvalidOperationException("Hand cannot be null");
         }

         // Draw a card (if available) and apply draw effect
         BaseCard drawnCard = Deck.DrawCard();
         if (drawnCard == null)
         {
            // Can't draw any more cards
            return false;
         }

         Logger.Log("Player " + this + " has drawn card " + drawnCard);
         Hand.AddCardToHand(drawnCard);

         return true;
      }
      #endregion

      #region Turn management
      /// <summary>
      /// Ends the turn. Called after ths player is no longer the active one.
      /// </summary>
      public virtual void EndTurn()
      {
         Logger.Log("End turn of player " + this);
      }

      /// <summary>
      /// Called after making this player the active one.
      /// 
      /// Draws a card from the deck.
      /// </summary>
      public virtual void StartTurn()
      {
         Logger.Log("Start turn of player " + this);
         DrawCard();
      }
      #endregion

      #region Win/Lose management
      /// <summary>
      /// Give up this match. Match will end after this.
      /// </summary>
      public virtual void Concede()
      { 
         Logger.Log(this + " has conceded this match.");

         ActiveMatch?.Concede(this);
      }

      /// <summary>
      /// This player is victorious. Match has already ended at this point.
      /// </summary>
      public virtual void Victorious()
      { 
         Logger.Log(this + " has won this match.");
      }

      /// <summary>
      /// This player is defeated. Match has already ended at this point.
      /// </summary>
      public virtual void Defeated()
      {
         Logger.Log(this + " has lost this match.");
      }
      #endregion

      /// <summary>
      /// Logs the contents of the hand to the logger instance.
      /// </summary>
      public void LogHand()
      {
         Logger.Log(this + " hand:");
         foreach (var card in Hand.Cards)
         {
            Logger.Log(Hand.Cards.IndexOf(card) + ": " + card.ToString());
         }
      }

      public override string ToString()
      {
         return string.Format("[BasePlayer: Name={2}, Deck={0}, Hand={1}]", Deck, Hand, Name);
      }
   }
}

