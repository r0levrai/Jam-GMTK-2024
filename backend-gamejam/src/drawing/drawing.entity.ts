import { Reaction } from "src/reaction/reaction.entity";
import { BaseEntity, Column, CreateDateColumn, Entity, OneToMany, PrimaryGeneratedColumn } from "typeorm";



export class Vector3 {
    x: number;
    y: number;
    z: number;
}

export class Points {
    points: Vector3[];
}


@Entity()
export class Drawing extends BaseEntity {
    @PrimaryGeneratedColumn()
    id: number;

    @CreateDateColumn()
    createdDate: Date;

    @Column("json")
    linesPoints: Points[];

    @Column("simple-array")
    linesWidth: number[];

    @Column("simple-array")
    linesColorIndex: number[];

    @Column()
    userName: string;

    @Column()
    drawingName: string;

    @Column()
    background: string;

    @Column()
    score: number;

    @OneToMany(() => Reaction, reaction => reaction.drawing)
    reactions: Reaction[];
}