import BoundingBox from './bounding-box';

export default class AzureTranscription {
    /**
     *
     */
    constructor(
        public BoundingBox: BoundingBox,
        public Text: string
    ) {}
}