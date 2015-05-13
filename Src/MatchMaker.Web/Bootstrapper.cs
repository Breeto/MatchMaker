using System.Collections.Generic;
using MatchMaker.Data;
using MatchMaker.MatchMaking;
using MatchMaker.MatchMaking.MatchHistory;
using MatchMaker.MatchMaking.Ratings;
using MatchMaker.Testing;
using MatchMaker.Web.Shared;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Authentication.Token;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;


namespace MatchMaker.Web
{

	/// <summary>
	/// Performs one-time and per request configuration for Nancy.
	/// </summary>
	public class Bootstrapper : DefaultNancyBootstrapper
	{

		protected override void ApplicationStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			base.ApplicationStartup( container, pipelines );

			Conventions.StaticContentsConventions.Add( StaticContentConventionBuilder.AddDirectory( "StaticContent" ) );

//			container.Register( typeof ( IDatabase ), typeof ( MongoDB.Database ) );
 			container.Register( typeof ( IDatabase ), typeof ( TestDatabase ) );
			container.Register(typeof(Settings), App.Settings);

			InitializeMatchupProposers( container );

			Nancy.Session.CookieBasedSessions.Enable( pipelines );
			System.Console.WriteLine("Hi from Bootstrapper: ApplicationStartup");
		}


		private static void InitializeMatchupProposers( TinyIoCContainer container )
		{
			container.Register( typeof ( IMatchupProposer ), typeof ( LimitedLinkMatchupProposer ), typeof ( LimitedLinkMatchupProposer ).Name );
			container.Register( typeof ( IMatchupProposer ), typeof ( RatingsMatchupProposer ), typeof ( RatingsMatchupProposer ).Name );

			var limitedLinkMatchupProposer = new LimitedLinkMatchupProposer( container.Resolve<IDatabase>(), container.Resolve<Settings>() );
			var ratingsMatchupProposer = new RatingsMatchupProposer( container.Resolve<IDatabase>(), container.Resolve<Settings>() );
			var winPercentageMatchupProposer = new WinPercentageMatchupProposer( container.Resolve<IDatabase>() );
// 			var comboProposer = new ComboMatchupProposer( new List<IMatchupProposer>
// 			{
// 				limitedLinkMatchupProposer,
// 				ratingsMatchupProposer
// 			} );
			var tieredMatchupProposer = new TieredMatchupProposer( container.Resolve<IDatabase>(), container.Resolve<Settings>() );

// 			container.Register( typeof ( IMatchupProposer ), comboProposer, comboProposer.GetType().Name );
			container.Register( typeof ( IMatchupProposer ), limitedLinkMatchupProposer, limitedLinkMatchupProposer.GetType().Name );
			container.Register( typeof ( IMatchupProposer ), ratingsMatchupProposer, ratingsMatchupProposer.GetType().Name );
			container.Register( typeof ( IMatchupProposer ), tieredMatchupProposer, tieredMatchupProposer.GetType().Name );
			container.Register( typeof ( IMatchupProposer ), winPercentageMatchupProposer, winPercentageMatchupProposer.GetType().Name );

			App.MatchupProposer = ratingsMatchupProposer;
			System.Console.WriteLine("Hi from InitializeMatchupProposers");
		}


		protected override void RequestStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
		{
			System.Console.WriteLine("Hi from Bootstrapper: RequestStartup 1");
			
			base.RequestStartup(container, pipelines, context);

			System.Console.WriteLine("Hi from Bootstrapper: RequestStartup 2");
			var formsAuthenticationConfiguration = new FormsAuthenticationConfiguration
			{
				RedirectUrl = Route.Login,
				UserMapper = container.Resolve<IUserMapper>(),
			};
			FormsAuthentication.Enable( pipelines, formsAuthenticationConfiguration );
			System.Console.WriteLine("Hi from Bootstrapper: RequestStartup 3");
			
			LayoutViewBagInitializer.Enable( pipelines, container.Resolve<IDatabase>() );
		}


		protected override DiagnosticsConfiguration DiagnosticsConfiguration
		{
			get
			{
				return new DiagnosticsConfiguration {Password = @"asdfasdf"};
			}
		}

	}

}

