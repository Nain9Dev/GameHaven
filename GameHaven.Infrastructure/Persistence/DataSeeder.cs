using GameHaven.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace GameHaven.Infrastructure.Persistence;

public static class GameExtensions 
{
    public static Game ApplyDiscountAndReturn(this Game game, int percentage) 
    {
        game.ApplyDiscount(percentage);
        return game;
    }
}

public static class DataSeeder
{
    public static async Task SeedAsync(GameHavenDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Games.AnyAsync())
            return;

        var games = new List<Game>
        {
            new Game("D e a t h I n P r o g r e s s", "An amazing adventure awaits in D e a t h I n P r o g r e s s.", 8.99m, "Studio 79", "Studio 79", DateTime.UtcNow.AddDays(-236), "001-DeathInProgress.png"),
            new Game("S h o r t H i k e", "An amazing adventure awaits in S h o r t H i k e.", 16.99m, "Studio 85", "Studio 85", DateTime.UtcNow.AddDays(-769), "002-ShortHike.png").ApplyDiscountAndReturn(37),
            new Game("H o l o C u r e", "An amazing adventure awaits in H o l o C u r e.", 30.99m, "Studio 9", "Studio 9", DateTime.UtcNow.AddDays(-746), "003-HoloCure.png").ApplyDiscountAndReturn(38),
            new Game("M e g a B o o t h", "An amazing adventure awaits in M e g a B o o t h.", 40.99m, "Studio 8", "Studio 8", DateTime.UtcNow.AddDays(-762), "004-MegaBooth.png"),
            new Game("S i x C a t s U n d e r", "An amazing adventure awaits in S i x C a t s U n d e r.", 26.99m, "Studio 13", "Studio 13", DateTime.UtcNow.AddDays(-677), "005-SixCatsUnder.png"),
            new Game("O u r L i f e", "An amazing adventure awaits in O u r L i f e.", 18.99m, "Studio 40", "Studio 40", DateTime.UtcNow.AddDays(-245), "006-OurLife.png").ApplyDiscountAndReturn(32),
            new Game("T h e G e n e r a l", "An amazing adventure awaits in T h e G e n e r a l.", 16.99m, "Studio 41", "Studio 41", DateTime.UtcNow.AddDays(-555), "007-TheGeneral.png").ApplyDiscountAndReturn(17),
            new Game("R o m e o", "An amazing adventure awaits in R o m e o.", 14.99m, "Studio 83", "Studio 83", DateTime.UtcNow.AddDays(-540), "008-Romeo.png"),
            new Game("N i g h t I n T h e W o o d s", "An amazing adventure awaits in N i g h t I n T h e W o o d s.", 35.99m, "Studio 35", "Studio 35", DateTime.UtcNow.AddDays(-761), "009-NightInTheWoods.png").ApplyDiscountAndReturn(36),
            new Game("V i n c e n t", "An amazing adventure awaits in V i n c e n t.", 41.99m, "Studio 29", "Studio 29", DateTime.UtcNow.AddDays(-292), "010-Vincent.png").ApplyDiscountAndReturn(17),
            new Game("W h e n T h e N i g h t C o m e s", "An amazing adventure awaits in W h e n T h e N i g h t C o m e s.", 47.99m, "Studio 20", "Studio 20", DateTime.UtcNow.AddDays(-418), "011-WhenTheNightComes.png"),
            new Game("M i n d u s t r y", "An amazing adventure awaits in M i n d u s t r y.", 47.99m, "Studio 51", "Studio 51", DateTime.UtcNow.AddDays(-345), "012-Mindustry.png").ApplyDiscountAndReturn(35),
            new Game("C i n d e r e l l a", "An amazing adventure awaits in C i n d e r e l l a.", 26.99m, "Studio 54", "Studio 54", DateTime.UtcNow.AddDays(-646), "013-Cinderella.png").ApplyDiscountAndReturn(19),
            new Game("M i s s e d M e s s a g e s", "An amazing adventure awaits in M i s s e d M e s s a g e s.", 38.99m, "Studio 50", "Studio 50", DateTime.UtcNow.AddDays(-930), "014-MissedMessages.png"),
            new Game("E b o n L i g h t", "An amazing adventure awaits in E b o n L i g h t.", 19.99m, "Studio 53", "Studio 53", DateTime.UtcNow.AddDays(-884), "015-EbonLight.png"),
            new Game("W a y F a r e r", "An amazing adventure awaits in W a y F a r e r.", 23.99m, "Studio 11", "Studio 11", DateTime.UtcNow.AddDays(-104), "016-WayFarer.png"),
            new Game("S c o u t", "An amazing adventure awaits in S c o u t.", 6.99m, "Studio 72", "Studio 72", DateTime.UtcNow.AddDays(-435), "017-Scout.png"),
            new Game("V a m p i r e S u r v i v o r s", "An amazing adventure awaits in V a m p i r e S u r v i v o r s.", 26.99m, "Studio 29", "Studio 29", DateTime.UtcNow.AddDays(-703), "018-VampireSurvivors.png"),
            new Game("B l o o d b o r n e", "An amazing adventure awaits in B l o o d b o r n e.", 48.99m, "Studio 5", "Studio 5", DateTime.UtcNow.AddDays(-988), "019-Bloodborne.png"),
            new Game("O b s c u r a", "An amazing adventure awaits in O b s c u r a.", 8.99m, "Studio 67", "Studio 67", DateTime.UtcNow.AddDays(-521), "020-Obscura.png"),
            new Game("S p e a k e r", "An amazing adventure awaits in S p e a k e r.", 32.99m, "Studio 3", "Studio 3", DateTime.UtcNow.AddDays(-694), "021-Speaker.png"),
            new Game("O n e S h o t", "An amazing adventure awaits in O n e S h o t.", 10.99m, "Studio 9", "Studio 9", DateTime.UtcNow.AddDays(-258), "022-OneShot.png"),
            new Game("D u c k S i m u l a t o r", "An amazing adventure awaits in D u c k S i m u l a t o r.", 25.99m, "Studio 36", "Studio 36", DateTime.UtcNow.AddDays(-670), "023-DuckSimulator.png"),
            new Game("S u p e r s t i t i o n", "An amazing adventure awaits in S u p e r s t i t i o n.", 43.99m, "Studio 89", "Studio 89", DateTime.UtcNow.AddDays(-904), "024-Superstition.png").ApplyDiscountAndReturn(34),
            new Game("D e v i l E x p r e s s", "An amazing adventure awaits in D e v i l E x p r e s s.", 42.99m, "Studio 48", "Studio 48", DateTime.UtcNow.AddDays(-284), "025-DevilExpress.png"),
            new Game("O n e N i g h t", "An amazing adventure awaits in O n e N i g h t.", 55.99m, "Studio 99", "Studio 99", DateTime.UtcNow.AddDays(-670), "026-OneNight.png"),
            new Game("T o u c h S t a r v e d", "An amazing adventure awaits in T o u c h S t a r v e d.", 34.99m, "Studio 85", "Studio 85", DateTime.UtcNow.AddDays(-997), "027-TouchStarved.png"),
            new Game("D e m o n S l a y e r", "An amazing adventure awaits in D e m o n S l a y e r.", 21.99m, "Studio 87", "Studio 87", DateTime.UtcNow.AddDays(-896), "028-DemonSlayer.png"),
            new Game("S t a r r y F l o w e r s", "An amazing adventure awaits in S t a r r y F l o w e r s.", 8.99m, "Studio 39", "Studio 39", DateTime.UtcNow.AddDays(-99), "029-StarryFlowers.png"),
            new Game("T w i l i c h t", "An amazing adventure awaits in T w i l i c h t.", 50.99m, "Studio 99", "Studio 99", DateTime.UtcNow.AddDays(-104), "030-Twilicht.png").ApplyDiscountAndReturn(36),
            new Game("P o c k e t M i r r o r", "An amazing adventure awaits in P o c k e t M i r r o r.", 8.99m, "Studio 4", "Studio 4", DateTime.UtcNow.AddDays(-529), "031-PocketMirror.png"),
            new Game("S a i n t S p e l l' s", "An amazing adventure awaits in S a i n t S p e l l' s.", 59.99m, "Studio 89", "Studio 89", DateTime.UtcNow.AddDays(-902), "032-SaintSpell's.png"),
            new Game("R e u n i o n", "An amazing adventure awaits in R e u n i o n.", 9.99m, "Studio 38", "Studio 38", DateTime.UtcNow.AddDays(-432), "033-Reunion.png"),
            new Game("B a l d i' s", "An amazing adventure awaits in B a l d i' s.", 18.99m, "Studio 58", "Studio 58", DateTime.UtcNow.AddDays(-757), "034-Baldi's.png"),
            new Game("G o o d M o r n i n g", "An amazing adventure awaits in G o o d M o r n i n g.", 6.99m, "Studio 57", "Studio 57", DateTime.UtcNow.AddDays(-506), "035-GoodMorning.png"),
            new Game("M o r t i c i a n s", "An amazing adventure awaits in M o r t i c i a n s.", 19.99m, "Studio 9", "Studio 9", DateTime.UtcNow.AddDays(-97), "036-Morticians.png"),
            new Game("F r a n k e n", "An amazing adventure awaits in F r a n k e n.", 47.99m, "Studio 64", "Studio 64", DateTime.UtcNow.AddDays(-541), "037-Franken.png"),
            new Game("C o n t r a c t D e m o n", "An amazing adventure awaits in C o n t r a c t D e m o n.", 32.99m, "Studio 9", "Studio 9", DateTime.UtcNow.AddDays(-539), "038-ContractDemon.png"),
            new Game("V e r d e u s", "An amazing adventure awaits in V e r d e u s.", 34.99m, "Studio 18", "Studio 18", DateTime.UtcNow.AddDays(-644), "039-Verdeus.png").ApplyDiscountAndReturn(48),
            new Game("P l a n t", "An amazing adventure awaits in P l a n t.", 12.99m, "Studio 54", "Studio 54", DateTime.UtcNow.AddDays(-95), "040-Plant.png").ApplyDiscountAndReturn(24),
            new Game("Y o u L e f t M e", "An amazing adventure awaits in Y o u L e f t M e.", 16.99m, "Studio 49", "Studio 49", DateTime.UtcNow.AddDays(-464), "041-YouLeftMe.png"),
            new Game("A r i a s S t o r y", "An amazing adventure awaits in A r i a s S t o r y.", 50.99m, "Studio 81", "Studio 81", DateTime.UtcNow.AddDays(-342), "042-AriasStory.png"),
            new Game("T h e C o f f i n", "An amazing adventure awaits in T h e C o f f i n.", 48.99m, "Studio 43", "Studio 43", DateTime.UtcNow.AddDays(-843), "043-TheCoffin.png").ApplyDiscountAndReturn(16),
            new Game("B l a c k O u t", "An amazing adventure awaits in B l a c k O u t.", 58.99m, "Studio 55", "Studio 55", DateTime.UtcNow.AddDays(-755), "044-BlackOut.png"),
            new Game("O f f D a y", "An amazing adventure awaits in O f f D a y.", 24.99m, "Studio 40", "Studio 40", DateTime.UtcNow.AddDays(-397), "045-OffDay.png"),
            new Game("T r a p p e d", "An amazing adventure awaits in T r a p p e d.", 59.99m, "Studio 70", "Studio 70", DateTime.UtcNow.AddDays(-888), "046-Trapped.png"),
            new Game("G r i m m", "An amazing adventure awaits in G r i m m.", 8.99m, "Studio 73", "Studio 73", DateTime.UtcNow.AddDays(-114), "047-Grimm.png"),
            new Game("M i o n i g h t", "An amazing adventure awaits in M i o n i g h t.", 53.99m, "Studio 3", "Studio 3", DateTime.UtcNow.AddDays(-143), "048-Mionight.png"),
            new Game("D i c e y D u n g e o n s", "An amazing adventure awaits in D i c e y D u n g e o n s.", 14.99m, "Studio 87", "Studio 87", DateTime.UtcNow.AddDays(-310), "049-DiceyDungeons.png").ApplyDiscountAndReturn(37),
            new Game("H u m a n H e a r t h", "An amazing adventure awaits in H u m a n H e a r t h.", 49.99m, "Studio 68", "Studio 68", DateTime.UtcNow.AddDays(-164), "050-HumanHearth.png"),
            new Game("S t e a d f a s t", "An amazing adventure awaits in S t e a d f a s t.", 20.99m, "Studio 25", "Studio 25", DateTime.UtcNow.AddDays(-337), "051-Steadfast.png").ApplyDiscountAndReturn(25),
            new Game("C r a z y", "An amazing adventure awaits in C r a z y.", 52.99m, "Studio 40", "Studio 40", DateTime.UtcNow.AddDays(-386), "052-Crazy.png"),
            new Game("G a c h a v e r s e", "An amazing adventure awaits in G a c h a v e r s e.", 15.99m, "Studio 56", "Studio 56", DateTime.UtcNow.AddDays(-209), "053-Gachaverse.png").ApplyDiscountAndReturn(37),
            new Game("C a f e I n T h e L o u d s", "An amazing adventure awaits in C a f e I n T h e L o u d s.", 47.99m, "Studio 74", "Studio 74", DateTime.UtcNow.AddDays(-770), "054-CafeInTheLouds.png"),
            new Game("S n a g g e m o n", "An amazing adventure awaits in S n a g g e m o n.", 9.99m, "Studio 21", "Studio 21", DateTime.UtcNow.AddDays(-911), "055-Snaggemon.png"),
            new Game("T o d a y", "An amazing adventure awaits in T o d a y.", 27.99m, "Studio 45", "Studio 45", DateTime.UtcNow.AddDays(-677), "056-Today.png"),
            new Game("D o o m", "An amazing adventure awaits in D o o m.", 10.99m, "Studio 56", "Studio 56", DateTime.UtcNow.AddDays(-542), "057-Doom.png").ApplyDiscountAndReturn(27),
            new Game("L o v e T h e G u a r d", "An amazing adventure awaits in L o v e T h e G u a r d.", 44.99m, "Studio 40", "Studio 40", DateTime.UtcNow.AddDays(-709), "058-LoveTheGuard.png"),
            new Game("L a d y s C h o i c e", "An amazing adventure awaits in L a d y s C h o i c e.", 30.99m, "Studio 83", "Studio 83", DateTime.UtcNow.AddDays(-720), "059-LadysChoice.png"),
            new Game("A r c a d e s S p i r i t s", "An amazing adventure awaits in A r c a d e s S p i r i t s.", 52.99m, "Studio 67", "Studio 67", DateTime.UtcNow.AddDays(-845), "060-ArcadesSpirits.png").ApplyDiscountAndReturn(29),
            new Game("A w a k e n i n g", "An amazing adventure awaits in A w a k e n i n g.", 44.99m, "Studio 44", "Studio 44", DateTime.UtcNow.AddDays(-665), "061-Awakening.png"),
            new Game("I n v i t e M e I n", "An amazing adventure awaits in I n v i t e M e I n.", 16.99m, "Studio 95", "Studio 95", DateTime.UtcNow.AddDays(-607), "062-InviteMeIn.png").ApplyDiscountAndReturn(11),
            new Game("Z o r l o k", "An amazing adventure awaits in Z o r l o k.", 7.99m, "Studio 93", "Studio 93", DateTime.UtcNow.AddDays(-847), "063-Zorlok.png"),
            new Game("P u r r f", "An amazing adventure awaits in P u r r f.", 25.99m, "Studio 38", "Studio 38", DateTime.UtcNow.AddDays(-764), "064-Purrf.png"),
            new Game("R o y a l O r d e r", "An amazing adventure awaits in R o y a l O r d e r.", 51.99m, "Studio 33", "Studio 33", DateTime.UtcNow.AddDays(-845), "065-RoyalOrder.png").ApplyDiscountAndReturn(35),
            new Game("C r y p t i d", "An amazing adventure awaits in C r y p t i d.", 22.99m, "Studio 47", "Studio 47", DateTime.UtcNow.AddDays(-324), "066-Cryptid.png"),
            new Game("W i l d e r", "An amazing adventure awaits in W i l d e r.", 55.99m, "Studio 8", "Studio 8", DateTime.UtcNow.AddDays(-216), "067-Wilder.png"),
            new Game("R e t u r n", "An amazing adventure awaits in R e t u r n.", 34.99m, "Studio 26", "Studio 26", DateTime.UtcNow.AddDays(-554), "068-Return.png"),
            new Game("W i t c h", "An amazing adventure awaits in W i t c h.", 45.99m, "Studio 57", "Studio 57", DateTime.UtcNow.AddDays(-440), "069-Witch.png"),
            new Game("P l a t f o r m e r T o o l K i t", "An amazing adventure awaits in P l a t f o r m e r T o o l K i t.", 23.99m, "Studio 85", "Studio 85", DateTime.UtcNow.AddDays(-107), "070-PlatformerToolKit.png").ApplyDiscountAndReturn(29),
            new Game("R a f t", "An amazing adventure awaits in R a f t.", 23.99m, "Studio 30", "Studio 30", DateTime.UtcNow.AddDays(-610), "81-Raft.png"),
            new Game("U l t i m a t e S w e e t", "An amazing adventure awaits in U l t i m a t e S w e e t.", 29.99m, "Studio 12", "Studio 12", DateTime.UtcNow.AddDays(-99), "82-UltimateSweet.png"),
            new Game("v i s u a l n o v e l", "An amazing adventure awaits in v i s u a l n o v e l.", 50.99m, "Studio 64", "Studio 64", DateTime.UtcNow.AddDays(-204), "83-visualnovel.png"),
            new Game("r o g u e l i k e", "An amazing adventure awaits in r o g u e l i k e.", 52.99m, "Studio 13", "Studio 13", DateTime.UtcNow.AddDays(-166), "84-roguelike.png"),
            new Game("C e l e s t e", "An amazing adventure awaits in C e l e s t e.", 53.99m, "Studio 4", "Studio 4", DateTime.UtcNow.AddDays(-994), "85-Celeste.png"),
            new Game("G a v r i l", "An amazing adventure awaits in G a v r i l.", 26.99m, "Studio 52", "Studio 52", DateTime.UtcNow.AddDays(-965), "86-Gavril.png"),
            
        };

        await context.Games.AddRangeAsync(games);
        await context.SaveChangesAsync();
    }
}
