import { ToolbarItem } from "./ToolbarItem";
export declare class ToolbarButton {
    private toolbarItem;
    private clickHandler;
    constructor(toolbarItem: ToolbarItem, clickHandler?: (ev: MouseEvent, toolbarItem: ToolbarItem) => void);
    getElement: () => HTMLElement;
}
