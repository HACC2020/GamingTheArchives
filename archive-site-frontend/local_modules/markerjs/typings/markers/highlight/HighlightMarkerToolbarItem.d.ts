import { ToolbarItem } from "../../toolbar/ToolbarItem";
import { HighlightMarker } from "./HighlightMarker";
export declare class HighlightMarkerToolbarItem implements ToolbarItem {
    name: string;
    tooltipText: string;
    icon: string;
    markerType: typeof HighlightMarker;
}
