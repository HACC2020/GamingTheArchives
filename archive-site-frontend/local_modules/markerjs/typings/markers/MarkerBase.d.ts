import { MarkerBaseState } from "./MarkerBaseState";
export declare class MarkerBase {
    static createMarker: () => MarkerBase;
    markerTypeName: string;
    visual: SVGGElement;
    renderVisual: SVGGElement;
    onSelected: (marker: MarkerBase) => void;
    defs: SVGElement[];
    protected markerId: string;
    protected width: number;
    protected height: number;
    protected isActive: boolean;
    protected isResizing: boolean;
    protected previousMouseX: number;
    protected previousMouseY: number;
    protected previousState: MarkerBaseState;
    private isDragging;
    manipulate: (ev: MouseEvent) => void;
    endManipulation(): void;
    select(): void;
    deselect(): void;
    getState(): MarkerBaseState;
    restoreState(state: MarkerBaseState): void;
    protected setup(): void;
    protected addToVisual: (el: SVGElement) => void;
    protected addToRenderVisual: (el: SVGElement) => void;
    protected resize(x: number, y: number): void;
    protected onTouch(ev: TouchEvent): void;
    private mouseDown;
    private mouseUp;
    private mouseMove;
    private move;
}
