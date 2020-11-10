import { ToolbarItem } from "../../toolbar/ToolbarItem";
import { RectMarker } from "./RectMarker";
export declare class RectMarkerToolbarItem implements ToolbarItem {
    name: string;
    tooltipText: string;
    icon: string;
    markerType: typeof RectMarker;
}
