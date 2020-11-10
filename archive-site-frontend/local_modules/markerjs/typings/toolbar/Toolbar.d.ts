import { ToolbarItem } from "./ToolbarItem";
export declare class Toolbar {
    private toolbarItems;
    private toolbarUI;
    private clickHandler;
    constructor(toolbarItems: ToolbarItem[], clickHandler: (ev: MouseEvent, toolbarItem: ToolbarItem) => void);
    getUI: () => HTMLElement;
}
