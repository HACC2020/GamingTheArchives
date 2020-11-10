import { ToolbarItem } from "../../toolbar/ToolbarItem";
import { CoverMarker } from "./CoverMarker";
export declare class CoverMarkerToolbarItem implements ToolbarItem {
    name: string;
    tooltipText: string;
    icon: string;
    markerType: typeof CoverMarker;
}
