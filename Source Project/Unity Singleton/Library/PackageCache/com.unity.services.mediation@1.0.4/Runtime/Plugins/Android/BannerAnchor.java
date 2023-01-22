package com.unity3d.mediation;

import android.view.Gravity;
public enum BannerAnchor {
    TopCenter(0),
    TopLeft(1),
    TopRight(2),
    Center(3),
    MiddleLeft(4),
    MiddleRight(5),
    BottomCenter(6),
    BottomLeft(7),
    BottomRight(8),
    None(9),
    Default(0);

    final int value;
    BannerAnchor(int value) {
        this.value = value;
    }
    public int getValue() {
        return this.value;
    }

    public static BannerAnchor fromInteger(int anchor) {
        for (BannerAnchor anchorEnum : BannerAnchor.values()) {
            if (anchorEnum.value == anchor) {
                return anchorEnum;
            }
        }
        return BannerAnchor.Default;
    }

    public static int GetGravity(BannerAnchor anchor) {
        switch(anchor) {
            case TopCenter:
                return Gravity.TOP | Gravity.CENTER_HORIZONTAL;
            case TopLeft:
                return Gravity.TOP | Gravity.START;
            case TopRight:
                return Gravity.TOP | Gravity.END;
            case Center:
                return Gravity.CENTER_VERTICAL | Gravity.CENTER_HORIZONTAL;
            case MiddleLeft:
                return Gravity.CENTER_VERTICAL | Gravity.START;
            case MiddleRight:
                return Gravity.CENTER_VERTICAL | Gravity.END;
            case BottomCenter:
                return Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
            case BottomLeft:
            case None:
                return Gravity.BOTTOM | Gravity.START;
            case BottomRight:
                return Gravity.BOTTOM | Gravity.END;
            default:
                return GetGravity(BannerAnchor.Default);
        }
    }

    public boolean isLeftColumn() {
        return value == TopLeft.getValue() || value == MiddleLeft.getValue() || value == BottomLeft.getValue() || value == None.getValue();
    }

    public boolean isCenterColumn() {
        return value == TopCenter.getValue() || value == Center.getValue() || value == BottomCenter.getValue();
    }

    public boolean isRightColumn() {
        return value == TopRight.getValue() || value == MiddleRight.getValue() || value == BottomRight.getValue();
    }

    public boolean isTopRow() {
        return value == TopLeft.getValue() || value == TopCenter.getValue() || value == TopRight.getValue();
    }

    public boolean isMiddleRow() {
        return value == MiddleLeft.getValue() || value == Center.getValue() || value == MiddleRight.getValue();
    }

    public boolean isBottomRow() {
        return value == BottomLeft.getValue() || value == BottomCenter.getValue() || value == BottomRight.getValue() || value == None.getValue();
    }
}
