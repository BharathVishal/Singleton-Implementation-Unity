package com.unity3d.mediation;
import android.util.Log;
import android.app.Activity;
import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.view.ViewGroup;
import android.view.View;
import android.view.ViewTreeObserver;
import android.widget.FrameLayout;
import com.unity3d.mediation.BannerAdView;
import com.unity3d.mediation.BannerAdViewSize;
import com.unity3d.mediation.IBannerAdViewListener;
import com.unity3d.mediation.errors.LoadError;

public class BannerAd {
    Handler handler;
    BannerAdView bannerAdView;
    BannerAnchor bannerAnchor;
    Activity activity;

    public BannerAd(Activity activity, String adUnitId, BannerAdViewSize size, IBannerAdViewListener bannerListener) {
        this.handler = new Handler(Looper.getMainLooper());
        this.activity = activity;
        this.bannerAdView = new BannerAdView(activity);

        bannerAdView.setAdUnitId(adUnitId);
        bannerAdView.setSize(size);

        bannerAdView.setListener(new IBannerAdViewListener() {
            @Override
            public void onBannerAdViewLoaded(BannerAdView bannerAdView) {
                bannerListener.onBannerAdViewLoaded(bannerAdView);
            }

            @Override
            public void onBannerAdViewFailedLoad(BannerAdView bannerAdView, LoadError loadError, String errorMessage) {
                bannerListener.onBannerAdViewFailedLoad(bannerAdView, loadError, errorMessage);
            }

            @Override
            public void onBannerAdViewRefreshed(BannerAdView bannerAdView, LoadError loadError, String errorMessage) {
                bannerListener.onBannerAdViewRefreshed(bannerAdView, loadError, errorMessage);
            }

            @Override
            public void onBannerAdViewClicked(BannerAdView bannerAdView) {
                bannerListener.onBannerAdViewClicked(bannerAdView);
            }
        });
    }

    public void destroy() {
	    bannerAdView.destroy();
    }

    public void load() {
        bannerAdView.load();
    }

    public String getAdUnitId() {
        return bannerAdView.getAdUnitId();
    }

    public AdState getAdState() {
        return bannerAdView.getAdState();
    } 

    public void setPosition(int anchor, int offsetX, int offsetY) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(bannerAdView.getParent() == null) {
                    activity.addContentView(bannerAdView, new FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT));

                    bannerAdView.post(new Runnable() {
                        @Override
                        public void run() {
                            setPositionInternal(anchor, offsetX, offsetY);
                        }
                    });
                }
                else {
                    bannerAdView.post(new Runnable() {
                        @Override
                        public void run() {
                            setPositionInternal(anchor, offsetX, offsetY);
                        }
                    });
                }
            }
        });
    }

    private void setPositionInternal (int anchor, int offsetX, int offsetY) {
        FrameLayout.LayoutParams adLayoutParams = (FrameLayout.LayoutParams) bannerAdView.getLayoutParams();
        if(adLayoutParams == null) return;

        bannerAnchor = BannerAnchor.fromInteger(anchor);
        int gravity = BannerAnchor.GetGravity(bannerAnchor);

        adLayoutParams.gravity = gravity;

        if(bannerAnchor.isTopRow()) {
            adLayoutParams.topMargin = -offsetY;
        }
        else{
            adLayoutParams.bottomMargin = offsetY;
        }

        if(bannerAnchor.isRightColumn()){
            adLayoutParams.rightMargin = -offsetX;
        }
        else{
            adLayoutParams.leftMargin = offsetX;
        }

        bannerAdView.setLayoutParams(adLayoutParams);
    }

}

