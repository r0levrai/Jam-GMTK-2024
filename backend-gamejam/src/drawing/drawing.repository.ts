import { CustomRepository } from "src/database/typeorm-ex.decorator";
import { Repository } from "typeorm";
import { Drawing } from "./drawing.entity";

@CustomRepository(Drawing)
export class DrawingRepository extends Repository<Drawing> {   
  

    // Find all drawings by a specific user
    async findByUserName(userName: string): Promise<Drawing[]> {
        return await this.find({ where: { userName } });
    }

    async createDrawing(drawingData: Partial<Drawing>): Promise<Drawing> {
        const drawing = this.create(drawingData);
        return await this.save(drawing);
    }

    async updateDrawing(id: number, updateData: Partial<Drawing>): Promise<Drawing> {
        await this.update(id, updateData);
        return await this.findOne({ where: { id } });
    }

    async findDrawingById(id: number): Promise<Drawing> {
        return await this.findOne({ where: { id } });
    }

    async deleteDrawing(id: number): Promise<void> {
        await this.delete(id);
    }

    async findLast10Drawings(): Promise<Drawing[]> {
        try {
            const query = this.createQueryBuilder("drawing")
                .orderBy("drawing.createdDate", "DESC")
                .take(10);
    
            console.log("Executing query:", query.getSql());
    
            return await query.getMany();
        } catch (error) {
            console.error("Error fetching the last 10 drawings:", error);
            throw new Error("Could not fetch the last 10 drawings");
        }
    }
    
    async findFirst10Drawings(): Promise<Drawing[]> {
        return await this.find({
            order: {
                createdDate: "ASC"
            },
            take: 10
        });
    }

    async findRandomDrawings(limit: number): Promise<Drawing[]> {
        return await this.createQueryBuilder("drawing")
            .orderBy("RANDOM()")
            .limit(limit)
            .getMany();
    }

    
}