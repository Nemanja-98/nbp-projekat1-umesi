import { Comment } from './comment'

export class Post {

    constructor(
        public Id: number,
        public UserRef: string,
        public title: string,
        public Description: string,
        public Ingridients: string[],
        public Comments: Comment[]) { }
}
