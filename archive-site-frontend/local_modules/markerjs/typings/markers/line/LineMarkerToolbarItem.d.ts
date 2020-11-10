import { ToolbarItem } from "../../toolbar/ToolbarItem";
import { LineMarker } from "./LineMarker";
export declare class LineMarkerToolbarItem implements ToolbarItem {
    name: string;
    tooltipText: string;
    icon: string;
    markerType: typeof LineMarker;
}
