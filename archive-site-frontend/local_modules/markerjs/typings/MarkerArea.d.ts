import { MarkerBase } from "./markers/MarkerBase";
import Config from "./Config";
import { MarkerAreaState } from "./MarkerAreaState";
import { MarkerBaseState } from "./markers/MarkerBaseState";
export declare class MarkerArea {
    private target;
    private targetRoot;
    private renderAtNaturalSize;
    private markerColors;
    private strokeWidth;
    private renderImageType?;
    private renderImageQuality?;
    private renderMarkersOnly?;
    private showUi;
    private previousState?;
    private markerImage;
    private markerImageHolder;
    private defs;
    private targetRect;
    private width;
    private height;
    private markers;
    private activeMarker;
    private toolbar;
    private toolbarUI;
    private logoUI;
    private completeCallback;
    private cancelCallback;
    private toolbars;
    private scale;
    constructor(target: HTMLImageElement, config?: Config);
    show: (completeCallback: (dataUrl: string, state?: MarkerAreaState) => void, cancelCallback?: () => void) => void;
    open: () => void;
    render: (completeCallback: (dataUrl: string, state?: MarkerAreaState) => void, cancelCallback?: () => void) => void;
    close: () => void;
    addMarker: (markerType: typeof MarkerBase, previousState?: MarkerBaseState) => void;
    selectMarkerById: (markerId: string) => void;
    deleteActiveMarker: () => void;
    resetState: () => void;
    getState: () => MarkerAreaState;
    restoreSavedState: (savedState: MarkerAreaState) => void;
    private restoreState;
    private setTargetRect;
    private startRender;
    private attachEvents;
    private mouseDown;
    private mouseMove;
    private mouseUp;
    private initMarkerCanvas;
    private adjustUI;
    private adjustSize;
    private positionUI;
    private positionMarkerImage;
    private positionToolbar;
    private showUI;
    private setStyles;
    private toolbarClick;
    private selectMarker;
    private deleteMarker;
    private complete;
    private cancel;
    private renderFinished;
    private renderFinishedClose;
    private positionLogo;
    /**
     * NOTE:
     *
     * before removing or modifying this method please consider supporting marker.js
     * by visiting https://markerjs.com/#price for details
     *
     * thank you!
     */
    private addLogo;
}
