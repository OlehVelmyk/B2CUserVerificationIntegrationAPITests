using WX.B2C.Risks.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Providers;

internal class TurnoverProvider
{
    private static readonly Guid EEAFirstThresholdTriggerVariantId = new("25540161-4CCB-4C03-9458-62CD8DAD0BFA");
    private static readonly Guid EEASecondThresholdTriggerVariantId = new("9BD82EB1-4649-460A-98B3-491A2C79A509");
    private static readonly Guid EEARepeatThresholdTriggerVariantId = new("E1FFB995-72CB-429E-99B2-DB4282DA6526");
    private static readonly Guid APACFirstThresholdTriggerVariantId = new("2B6467DB-BA15-42E5-9B3D-A4DF4FAE2475");
    private static readonly Guid APACSecondThresholdTriggerVariantId = new("E8671ADD-B3F4-4C61-ABB8-310B31AF9C2E");
    private static readonly Guid PHFirstThresholdTriggerVariantId = new("139C0711-AE5D-42A5-9CE4-3DE2877D4D86");
    private static readonly Guid PHSecondThresholdTriggerVariantId = new("13FA5763-CD13-4C38-9586-81F6C80183CC");

    private static readonly Guid GbFirstThresholdTriggerVariantId = new("044529B5-D983-4F57-8DBA-16FEBF264052");
    private static readonly Guid GbSecondThresholdTriggerVariantId = new("F623D69C-F636-4DAC-923A-5BE2C3779D7B");
    private static readonly Guid GbRepeatingThresholdTriggerVariantId = new("6F762B87-112E-43C5-B311-C32CDF723D63");

    private static readonly Guid RoWFirstThresholdTriggerVariantId = new("F7AEF22C-0638-4322-97FD-E5DA05918D1F");
    private static readonly Guid RoWSecondThresholdTriggerVariantId = new("47EAA9B2-0FE0-41E0-A1E8-B502F48A9C23");
    private static readonly Guid RoWThirdThresholdTriggerVariantId = new("C8C1ECF5-C784-41BB-8FAD-C30CFAC68918");
    private static readonly Guid RoWFourthThresholdTriggerVariantId = new("82CC6921-9263-442A-B022-00A8D1D5A4BF");

    public (Guid variantId, int turnoverAmount) GetTurnoverInfo(
        RiskRating riskRating,
        Region region,
        ThresholdInfo thresholdInfo)
    {
        var response = (region, riskRating, turnoverInfo: thresholdInfo) switch
        {
            (Region.Eea, RiskRating.Low, ThresholdInfo.First)                              => (EEAFirstThresholdTriggerVariantId, 60_000),
            (Region.Eea, RiskRating.Medium, ThresholdInfo.First)                           => (EEAFirstThresholdTriggerVariantId, 30_000),
            (Region.Eea, RiskRating.Low, ThresholdInfo.Second)                             => (EEASecondThresholdTriggerVariantId, 75_000),
            (Region.Eea, RiskRating.Medium, ThresholdInfo.Second)                          => (EEASecondThresholdTriggerVariantId, 60_000),
            (Region.Eea, RiskRating.Low, ThresholdInfo.RepeatingTurnoverThresholdStep)     => (EEARepeatThresholdTriggerVariantId, 75_000),
            (Region.Eea, RiskRating.Medium, ThresholdInfo.RepeatingTurnoverThresholdStep)  => (EEARepeatThresholdTriggerVariantId, 60_000),
            (Region.Eea, RiskRating.High, ThresholdInfo.RepeatingTurnoverInitialThreshold) => (EEARepeatThresholdTriggerVariantId, 15_000),
            (Region.Eea, RiskRating.High, ThresholdInfo.RepeatingTurnoverThresholdStep)    => (EEARepeatThresholdTriggerVariantId, 30_000),
            (Region.Eea, RiskRating.ExtraHigh, ThresholdInfo.RepeatingTurnoverInitialThreshold) => (EEARepeatThresholdTriggerVariantId, 15_000),
            (Region.Eea, RiskRating.ExtraHigh, ThresholdInfo.RepeatingTurnoverThresholdStep)    => (EEARepeatThresholdTriggerVariantId, 30_000),

            (Region.Apac, RiskRating.High, ThresholdInfo.First)  => (APACFirstThresholdTriggerVariantId, 2_300),
            (Region.Apac, RiskRating.High, ThresholdInfo.Second) => (APACSecondThresholdTriggerVariantId, 2_900),
            (Region.Ph, RiskRating.High, ThresholdInfo.First)    => (PHFirstThresholdTriggerVariantId, 400),
            (Region.Ph, RiskRating.High, ThresholdInfo.Second)   => (PHSecondThresholdTriggerVariantId, 550),

            (Region.Gb, RiskRating.Low, ThresholdInfo.First)                                   => (GbFirstThresholdTriggerVariantId, 60_000),
            (Region.Gb, RiskRating.Medium, ThresholdInfo.First)                                => (GbFirstThresholdTriggerVariantId, 30_000),
            (Region.Gb, RiskRating.Low, ThresholdInfo.Second)                                  => (GbSecondThresholdTriggerVariantId, 75_000),
            (Region.Gb, RiskRating.Medium, ThresholdInfo.Second)                               => (GbSecondThresholdTriggerVariantId, 60_000),
            (Region.Gb, RiskRating.Low, ThresholdInfo.RepeatingTurnoverThresholdStep)          => (GbRepeatingThresholdTriggerVariantId, 75_000),
            (Region.Gb, RiskRating.Medium, ThresholdInfo.RepeatingTurnoverThresholdStep)       => (GbRepeatingThresholdTriggerVariantId, 60_000),
            (Region.Gb, RiskRating.High, ThresholdInfo.RepeatingTurnoverThresholdStep)         => (GbRepeatingThresholdTriggerVariantId, 30_000),
            (Region.Gb, RiskRating.High, ThresholdInfo.RepeatingTurnoverInitialThreshold)      => (GbRepeatingThresholdTriggerVariantId, 15_000),
            (Region.Gb, RiskRating.ExtraHigh, ThresholdInfo.RepeatingTurnoverThresholdStep)    => (GbRepeatingThresholdTriggerVariantId, 30_000),
            (Region.Gb, RiskRating.ExtraHigh, ThresholdInfo.RepeatingTurnoverInitialThreshold) => (GbRepeatingThresholdTriggerVariantId, 15_000),

            (Region.RoW, RiskRating.High, ThresholdInfo.First)    => (RoWFirstThresholdTriggerVariantId, 4_000),
            (Region.RoW, RiskRating.High, ThresholdInfo.Second)   => (RoWSecondThresholdTriggerVariantId, 5_000),
            (Region.RoW, RiskRating.Low, ThresholdInfo.Third)     => (RoWThirdThresholdTriggerVariantId, 100_000),
            (Region.RoW, RiskRating.Medium, ThresholdInfo.Third)  => (RoWThirdThresholdTriggerVariantId, 75_000),
            (Region.RoW, RiskRating.High, ThresholdInfo.Third)    => (RoWThirdThresholdTriggerVariantId, 50_000),
            (Region.RoW, RiskRating.Low, ThresholdInfo.Fourth)    => (RoWFourthThresholdTriggerVariantId, 200_000),
            (Region.RoW, RiskRating.Medium, ThresholdInfo.Fourth) => (RoWFourthThresholdTriggerVariantId, 150_000),
            (Region.RoW, RiskRating.High, ThresholdInfo.Fourth)   => (RoWFourthThresholdTriggerVariantId, 100_000),
            _                                                     => throw new ArgumentOutOfRangeException($"{region} {riskRating} {thresholdInfo} turnover not found")
        };

        return response;
    }
}

internal enum Region
{
    Apac,
    Eea,
    Global,
    Ph,
    Gb,
    RoW
}

internal enum ThresholdInfo
{
    First,
    Second,
    Third,
    Fourth,
    RepeatingTurnoverInitialThreshold,
    RepeatingTurnoverThresholdStep
}
