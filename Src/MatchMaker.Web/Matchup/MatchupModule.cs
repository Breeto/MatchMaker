using System;
using System.Collections.Generic;
using System.Linq;
using MatchMaker.Common;
using MatchMaker.Data;
using MatchMaker.MatchMaking;
using MatchMaker.Web.Common;
using Nancy;
using Nancy.ModelBinding;


namespace MatchMaker.Web.Matchup
{

	public class MatchupModule : GameModule
	{

		private readonly IMatchupProposer[] proposers;


		public MatchupModule( IDatabase database, Settings settings, IEnumerable<IMatchupProposer> proposers ) : base( database )
		{
			this.proposers = proposers.ToArray();


			Get[Route.FindTeams] = o =>
			{
				var model = new FindTeamsModel()
				{
					Game = game,
					AvailablePlayers = database.GetPlayersForGame( game.Id ).OrderBy( player => player.Name ).ToArray(),
					AlgorithmName = App.MatchupProposer.GetType().Name,
				};

				return View[Route.FindTeams, model];
			};


			Get[Route.SetMatchupProposer] = o =>
			{
				var model = new SetMatchupProposerViewModel( proposers, App.MatchupProposer );

				return View[Route.SetMatchupProposer, model];
			};


			Post[Route.SetMatchupProposer] = o =>
			{
				var request = this.Bind<SetMatchupProposerRequest>();

				var matchupProposer = GetAlgorithm( request.SelectedAlgorithmTypeName );

				App.MatchupProposer = matchupProposer;

				return Response.AsRedirect( Route.FindTeams );
			};


			Get[Route.GetProposedMatchups] = o =>
			{
				var request = this.Bind<GetProposedMatchupsRequest>();

				var proposer = App.MatchupProposer;

				var response = new GetProposedMatchupsResponse();

				try
				{
					var gameKey = (string)o.gameKey;
					var game = database.GetGameByKey( gameKey );

					if ( null == request.PlayerIds || request.PlayerIds.Count < 2 )
					{
						throw new Exception( "Two or more players must be selected" );
					}
					var players = request.PlayerIds.Select( database.GetUserById ).ToArray();

					var proposedMatchups = proposer.GetMatchups( game, players ).OrderBy( matchup => matchup.Imbalance ).ToArray();
					proposedMatchups = CleanUp( proposedMatchups );

					response.ProposedMatchups = proposedMatchups;
				}
				catch ( Exception e )
				{
					response.ErrorMessage = e.Message;
				}

				return View[Route.GetProposedMatchups, response];
			};


			Post[Route.PresentMatchupForSaving] = o =>
			{
				var json = (string)Request.Form.matchup;
				var matchup = json.FromJson<ProposedMatchup>();

				return View[Route.SaveMatchupResult, new SaveMatchupResultViewModel(matchup, database.GetMaps(matchup.Game.Id))];
			};

			
			Post[Route.SaveMatchupResult] = o =>
			{
				var result = this.Bind<MatchupResult>();
				database.SaveMatchupResult( result );

				return Response.AsRedirect( Route.ViewResults );
			};


			Get[Route.ViewResults] = o =>
			{
				var game = database.GetGameByKey( o.gameKey );
				var matchupResults = database.GetMatchupResultsByGame( game.Id );
				var userMap = new IdentityMap<User>( database.GetUserById );

				var model = new List<MatchupResultViewModel>();
				foreach ( var matchupResult in matchupResults )
				{
					var map = database.GetMapById(matchupResult.MapId);
					model.Add(new MatchupResultViewModel(matchupResult, userMap, map));
				}

				return View[Route.ViewResults, model];
			};


			Get[Route.SelectPlayers] = o =>
			{
				var model = new SelectPlayersViewModel();

				var users = database.GetUsers();
				var playersForGame = database.GetPlayersForGame( this.game.Id );

				foreach ( var user in users )
				{
					model.SelectableUsers.Add( new Selectable<User>
					{
						Item = user,
						IsSelected = playersForGame.Any( user1 => user1.Id == user.Id ),
					} );
				}

				return View[Route.SelectPlayers, model];
			};


			Post[Route.SelectPlayers] = o =>
			{
				var request = this.Bind<SelectPlayersRequest>();

				database.SetActivePlayersForGame( game.Id, request.UserIds );

				return Response.AsRedirect( Route.FindTeams );
			};

		}


		private IMatchupProposer GetAlgorithm( string algorithmTypeName )
		{
			return proposers.First( proposer => proposer.GetType().FullName == algorithmTypeName );
		}


		private ProposedMatchup[] CleanUp( ProposedMatchup[] proposedMatchups )
		{
			foreach ( var proposedMatchup in proposedMatchups )
			{
				if ( proposedMatchup.Team1PredictedWinRatio < 0.5 )
				{
					proposedMatchup.SwapTeams();
				}
			}

			return proposedMatchups
				.OrderBy( matchup => matchup.Imbalance )
				.Take( App.Settings.MaxMatchupProposalsToPresent ).ToArray();
		}

	}
}


