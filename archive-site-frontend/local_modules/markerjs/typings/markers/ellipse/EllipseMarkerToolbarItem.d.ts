import { ToolbarItem } from "../../toolbar/ToolbarItem";
import { EllipseMarker } from "./EllipseMarker";
export declare class EllipseMarkerToolbarItem implements ToolbarItem {
    name: string;
    tooltipText: string;
    icon: string;
    markerType: typeof EllipseMarker;
}
