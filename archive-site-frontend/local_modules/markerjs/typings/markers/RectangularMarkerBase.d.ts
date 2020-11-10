import { MarkerBase } from "./MarkerBase";
export declare class RectangularMarkerBase extends MarkerBase {
    static createMarker: () => RectangularMarkerBase;
    protected MIN_SIZE: number;
    private controlBox;
    private readonly CB_DISTANCE;
    private controlRect;
    private controlGrips;
    private activeGrip;
    endManipulation(): void;
    select(): void;
    deselect(): void;
    protected setup(): void;
    protected resize(dx: number, dy: number): void;
    protected onTouch(ev: TouchEvent): void;
    private addControlBox;
    private adjustControlBox;
    private addControlGrips;
    private createGrip;
    private positionGrips;
    private positionGrip;
    private gripMouseDown;
    private gripMouseUp;
    private gripMouseMove;
}
