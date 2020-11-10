import { RectangularMarkerBase } from "../RectangularMarkerBase";
export declare class EllipseMarker extends RectangularMarkerBase {
    static createMarker: () => RectangularMarkerBase;
    constructor();
    private markerEllipse;
    protected setup(): void;
    protected resize(x: number, y: number): void;
}
