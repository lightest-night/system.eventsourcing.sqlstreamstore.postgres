CREATE TABLE IF NOT EXISTS __schema__.checkpoints (
    id              BIGINT          GENERATED ALWAYS AS IDENTITY NOT NULL,
    checkpoint_id   VARCHAR(500)    NOT NULL,
    checkpoint      BIGINT          NULL,
    CONSTRAINT pk_checkpoints PRIMARY KEY (id),
    CONSTRAINT uq_checkpoints_checkpoint_id UNIQUE (checkpoint_id)
);

CREATE INDEX IF NOT EXISTS ix_checkpoint_id
    ON __schema__.checkpoints (checkpoint_id);