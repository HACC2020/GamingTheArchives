import { ResizeGrip } from "./ResizeGrip";
export declare class RectangularMarkerGrips {
    topLeft: ResizeGrip;
    topCenter: ResizeGrip;
    topRight: ResizeGrip;
    centerLeft: ResizeGrip;
    centerRight: ResizeGrip;
    bottomLeft: ResizeGrip;
    bottomCenter: ResizeGrip;
    bottomRight: ResizeGrip;
    findGripByVisual: (gripVisual: SVGGraphicsElement) => ResizeGrip;
}
