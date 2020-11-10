import { MarkerBaseState } from './markers/MarkerBaseState';
import { MarkerBase } from './markers/MarkerBase';
export declare class MarkerAreaState {
    constructor(markers?: MarkerBase[]);
    markers?: MarkerBaseState[];
    getJSON(): string;
}
