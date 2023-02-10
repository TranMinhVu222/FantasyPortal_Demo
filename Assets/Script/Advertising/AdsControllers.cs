using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdsControllers: MonoBehaviour
{
    // Start is called before the first frame update
    public string appKey;
    private bool available;

    private void Start()
    {
        IronSource.Agent.shouldTrackNetworkState(true);
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdsClosedEvent;
    }

    public void Rewarded()
    {
        IronSource.Agent.showRewardedVideo();
    }

    void RewardedVideoAdsClosedEvent()
    {
        IronSource.Agent.init(appKey,IronSourceAdUnits.REWARDED_VIDEO);
        IronSource.Agent.shouldTrackNetworkState(true);
    }
    void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        bool rewardedVideoAvailability = available;
    }
}
