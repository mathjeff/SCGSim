using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// adds/removes resources from a player
namespace Games
{
    class ResourceEffect : TriggeredGameEffect<GameEffect>
    {
        // Fully-configurable constructor for a card that decides later on who to give resources to and how much
        public ResourceEffect(ValueProvider<Resource, Controlled> resourcesToGain_provider, ValueProvider<Writable_GamePlayer, Controlled> playerProvider)
        {
            this.Initialize(resourcesToGain_provider, playerProvider);
        }
        private void Initialize(ValueProvider<Resource, Controlled> resourcesToGain_provider, ValueProvider<Writable_GamePlayer, Controlled> playerProvider)
        {
            this.resourcesToGain_provider = resourcesToGain_provider;
            this.playerProvider = playerProvider;
        }
        public override TriggeredGameEffect<GameEffect> Clone(TriggeredGameEffect<GameEffect> outputType)
        {
            return new ResourceEffect(this.resourcesToGain_provider, this.playerProvider);
        }
        public override void Process(Game game)
        {
            Writable_GamePlayer writablePlayer = this.playerProvider.GetValue(this.Cause, game, (Writable_GamePlayer)null);

            Resource newResources = writablePlayer.CurrentResources.Plus(this.resourcesToGain_provider.GetValue(this.Cause, game, (Resource)null));
            if (newResources.IsValid)
                writablePlayer.CurrentResources = newResources;
            else
                writablePlayer.CurrentResources = new Resource(0);
        }
        public override string ToString(Game game)
        {
            Readable_GamePlayer player = this.playerProvider.GetValue(this.Cause, game, (Writable_GamePlayer)null);
            Resource bonusResources = this.resourcesToGain_provider.GetValue(this.Cause, game, (Resource)null);
            return player.ToString() + " gains " + bonusResources.ToString();
        }
        private ValueProvider<Writable_GamePlayer, Controlled> playerProvider;
        private ValueProvider<Resource, Controlled> resourcesToGain_provider;
    }
}
