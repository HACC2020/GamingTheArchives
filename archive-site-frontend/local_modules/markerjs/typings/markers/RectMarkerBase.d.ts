import { RectangularMarkerBase } from "./RectangularMarkerBase";
export declare class RectMarkerBase extends RectangularMarkerBase {
    static createMarker: () => RectMarkerBase;
    private markerRect;
    protected setup(): void;
    protected resize(x: number, y: number): void;
}
