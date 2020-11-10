import { ToolbarItem } from "../../toolbar/ToolbarItem";
import { ArrowMarker } from "./ArrowMarker";
export declare class ArrowMarkerToolbarItem implements ToolbarItem {
    name: string;
    tooltipText: string;
    icon: string;
    markerType: typeof ArrowMarker;
}
