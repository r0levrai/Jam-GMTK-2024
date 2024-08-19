import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ReactionRepository } from './reaction.repository';
import { DrawingRepository } from '../drawing/drawing.repository';
import { TypeOrmExModule } from 'src/database/typeorm-ex.module';

@Module({
    imports: [TypeOrmExModule.forCustomRepository([ReactionModule])],
    providers: [ReactionRepository],
    controllers: [],
    exports: [ReactionRepository],
})
export class ReactionModule {}