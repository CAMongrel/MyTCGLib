using System;
using System.Linq;
using System.Collections.Generic;

namespace MyTCGLib
{
   /// <summary>
   /// Instance of a single match between two or more runtime players.
   /// 
   /// Derived class may or may not abstract the variable amount of players into a concrete amount
   /// </summary>
   public class BaseMatch
   {
      protected List<BasePlayer> playerList;

      public int CurrentRound { get; protected set; }

      public bool IsRunning { get; protected set; }

      public BasePlayer ActivePlayer { get; protected set; }

      public Random Random { get; private set; }

      public BaseMatch(Random setRandom)
      {
         Random = setRandom;
         IsRunning = false;
         CurrentRound = 0;
         ActivePlayer = null;
         playerList = new List<BasePlayer>();
      }

      /// <summary>
      /// Prepares for the start of a match. Can be used to shuffle the deck or similar things.
      /// 
      /// Calls <c>PrepareStart</c>
      /// </summary>
      public virtual void PreStart()
      {
         PrepareStart();
      }

      /// <summary>
      /// Starts this match. This is directly followed by the first round of the match.
      /// 
      /// Calls <c>SwitchToRunState</c> and <c>StartNextRound</c>
      /// </summary>
      public virtual void Start()
      {
         SwitchToRunState();

         StartNextRound();
      }

      /// <summary>
      /// Base implementation simply sets IsRunning to true.
      /// 
      /// PrepareStart has already been called, but the first round hasn't started yet.
      /// </summary>
      protected virtual void SwitchToRunState()
      { 
         IsRunning = true;
      }

      /// <summary>
      /// Stuff to be done before starting the match (e.g. shuffling the deck, creating initial hand etc.)
      /// Base implementation sets the CurrentRound to zero, calls ClearPlayfield() and PreparePlayers()
      /// 
      /// Match will start with the first round after this.
      /// </summary>
      protected virtual void PrepareStart()
      {
         CurrentRound = 0;

         ClearPlayfield();
         PreparePlayers();
      }

      /// <summary>
      /// Called by PrepareStart. Base implementation does nothing.
      /// </summary>
      protected virtual void ClearPlayfield()
      {
         //
      }

      /// <summary>
      /// Called by PrepareStart. Base implementation does nothing.
      /// </summary>
      protected virtual void PreparePlayers()
      {
         //
      }

      /// <summary>
      /// Returns the player that should be activated next
      /// </summary>
      /// <returns>The next player.</returns>
      protected virtual BasePlayer GetNextPlayer()
      {
         // "CurrentRound" is one-based, but we don't want to start with the player at index 1, so we change CurrentRound to be zero-based
         int currentRound = CurrentRound - 1;
         return playerList[currentRound % playerList.Count];
      }

      /// <summary>
      /// Starts the next round, deactivating the previous player and activating the next
      /// </summary>
      public virtual void StartNextRound()
      {
         if (IsRunning == false)
         {
            // TODO: Log
            return;
         }

         DeactivatePlayer(ActivePlayer);

         CurrentRound++;

         ActivatePlayer(GetNextPlayer());
      }

      /// <summary>
      /// Stop this match. Sets IsRunning to false.
      /// </summary>
      public virtual void Stop()
      {
         IsRunning = false;
      }

      /// <summary>
      /// Deactivates the player. Called at the end of a round. Invokes "EndTurn" for that player.
      /// 
      /// CurrentRound still has the old value.
      /// </summary>
      /// <param name="player">The player to deactivate. May be null.</param>
      protected virtual void DeactivatePlayer(BasePlayer player)
      {
         if (player == null)
         {
            return;
         }

         player.EndTurn();
      }

      /// <summary>
      /// Activates the player. Called at the start of a round. Invokes "StartTurn" for that player.
      /// 
      /// CurrentRound now has the new value.
      /// </summary>
      /// <param name="player">Player. Must not be nil</param>
      protected virtual void ActivatePlayer(BasePlayer player)
      {
         if (player == null)
         {
            throw new ArgumentNullException(nameof(player));
         }

         ActivePlayer = player;

         ActivePlayer.StartTurn();
      }

      /// <summary>
      /// The player wants to concede this match. Match ends now.
      /// 
      /// All other players will be declared as winners.
      /// </summary>
      /// <param name="player">Player that will lose this match.</param>
      public virtual void Concede(BasePlayer player)
      {
         DeclareLoser(player);
      }

      /// <summary>
      /// The player is declared as loser of this match. Match ends now.
      /// 
      /// All other players will be declared as winners.
      /// </summary>
      /// <param name="player">Player that will lose this match.</param>
      public virtual void DeclareLoser(BasePlayer player)
      {
         Stop();

         List<BasePlayer> losers = new List<BasePlayer>();
         losers.Add(player);

         List<BasePlayer> winners = new List<BasePlayer>();
         winners.AddRange(from pl in playerList where pl != player select pl);

         foreach (var pl in losers)
         {
            pl.Defeated();
         }

         foreach (var pl in winners)
         {
            pl.Victorious();
         }
      }
   }
}
